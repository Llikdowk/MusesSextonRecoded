using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Game.PlayerComponents {
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class CharacterController : MonoBehaviour {

		public Vector3 Gravity = Physics.gravity;
		public float GravityMultiplier = 1.0f;
		[Range(0, 0.5f)] public float VerticalSkinWidth = 0.15f;
		[Range(0, 0.5f)] public float HorizontalSkinWidth = 0.15f;
		[Range(0, 1.0f)] public float SlopeInclinationAllowance = 0.1f;
		[Range(0, 1.0f)] public float StepAllowance = 0.0f;
		public float GrounderDistance = 2.0f;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private float _timeOnAir = 0.0f;
		private CharacterMovement _charMovement;

		private ActionManager<PlayerAction> _actions;

		private bool IsGrounded {
			get { return _isGrounded; }
			set {
				_isGrounded = value;
				if (_isGrounded) {
					_timeOnAir = 0.0f;
				}
			}
		}

		private bool _isGrounded = false;


		public void Start() {
			_actions = Player.GetInstance().Actions;
			int player = 1 << LayerMaskManager.Get(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();
		}

		private int CompareRayhits(RaycastHit x, RaycastHit y) {
			return x.distance.CompareTo(y.distance);
		}

		private int ClassifyTriggers(ref RaycastHit[] hits, ref RaycastHit[] triggers, ref int hitsLength) {
			int triggerCount = 0;

			for (int i = hitsLength-1; i >= 0; --i) {
				if (hits[i].collider.isTrigger) {
					for (int j = i; j < hitsLength - 1; ++j) {
						hits[i] = hits[i + 1];
					}
					--hitsLength;
					++triggerCount;
				}
			}

			return triggerCount;
		}

		// minor-to-major raycast distance ordered (insertion sort, faster than default quicksort for low capacity arrays)
		private void Sort(ref RaycastHit[] hits, int start, int length) {
			RaycastHit item, nextItem, targetItem, aux;
			for (int i = start; i < length - 1; ++i) {
				item = hits[i];
				nextItem = hits[i + 1];
				if (nextItem.distance < item.distance) {
					for (int j = start; j <= i; ++j) {
						targetItem = hits[j];
						if (nextItem.distance < targetItem.distance) {
							while (j <= i) {
								aux = hits[j+1];
								hits[j+1] = hits[j];
								hits[j] = nextItem;
								nextItem = aux;
								++j;
							}
						}
					}
				} 
			}
		}

		private const int MaxSimultaneousColliders = 16;
		private const int MaxSimultaneousTriggers = 16;
		private RaycastHit[] floorHits = new RaycastHit[MaxSimultaneousColliders];
		private RaycastHit[] triggers = new RaycastHit[MaxSimultaneousTriggers];
		public void Update() {
			Debug.DrawRay(transform.position, _charMovement.WorldDir, Color.cyan);
			Vector3 gravityForce = Vector3.zero;
			if (!IsGrounded) {
				gravityForce = Gravity * _timeOnAir * Time.deltaTime;
				_timeOnAir += Time.deltaTime;
			}
			Matrix4x4 M = _collider.transform.localToWorldMatrix;
			Vector3 capsuleHead = transform.position + M.MultiplyVector(_collider.center) +
			                      transform.up * (_collider.height / 2.0f - _collider.radius);
			Vector3 capsuleFeet = transform.position + M.MultiplyVector(_collider.center) -
			                      transform.up * (_collider.height / 2.0f - _collider.radius);

			
			CheckWalls(capsuleHead, capsuleFeet, transform.forward, PlayerAction.MoveForward); // TODO would be nice to have this independant of Action
			CheckWalls(capsuleHead, capsuleFeet, -transform.forward, PlayerAction.MoveBack);
			CheckWalls(capsuleHead, capsuleFeet, transform.right, PlayerAction.MoveRight);
			CheckWalls(capsuleHead, capsuleFeet, -transform.right, PlayerAction.MoveLeft);
			

			//CheckFloor
			int floorHitsCount = Physics.SphereCastNonAlloc(capsuleFeet, _collider.radius, -Vector3.up, floorHits, GrounderDistance, _layerMaskAllButPlayer);
			int collidersFound = 0;

			int triggerCount = ClassifyTriggers(ref floorHits, ref triggers, ref floorHitsCount);
			Sort(ref floorHits, 0, floorHitsCount);
			Debug.Log(triggerCount + " " + floorHitsCount);

			for (int i = 0; i < floorHitsCount; ++i) { 
				RaycastHit hit = floorHits[i];
				if (hit.collider.isTrigger) {
					//hit.collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
					continue;
				}
				++collidersFound;
				// overlapping collision
				if (hit.point == Vector3.zero) {
					transform.position = transform.position + -gravityForce + Vector3.up * VerticalSkinWidth / 2.0f;
					IsGrounded = true;
					break;
				}
				else if (hit.distance < VerticalSkinWidth) {
					IsGrounded = true;
					break;
				}
				else {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					transform.position += Vector3.down * (hit.distance - VerticalSkinWidth/2.0f);
					IsGrounded = true;
					break;
				}
			}




			if (collidersFound == 0) {
				IsGrounded = false;
			}

			if (!_isGrounded) {
				transform.position += gravityForce;
			}
			else {
				_timeOnAir = 0.0f;
			}

		}

		private void CheckWalls(Vector3 capsuleHead, Vector3 capsuleFeet, Vector3 dir, PlayerAction playerActionType) {
			int collidersFound = 0;
			Vector3 stepOffset = transform.up * StepAllowance;
			RaycastHit[] wallHits = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, dir,
				4 * HorizontalSkinWidth, _layerMaskAllButPlayer);
			foreach (RaycastHit hit in wallHits) {
				if (hit.collider.isTrigger) {
					//hit.collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
					continue;
				}
				++collidersFound;
				if (hit.point == Vector3.zero) {
					_charMovement.AddForce(hit.normal, _charMovement.StepMovement.magnitude + HorizontalSkinWidth / 2.0f);
				}
				else if (Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal)) >= (1 - SlopeInclinationAllowance)) {
					continue;
				}
				else if (hit.distance > HorizontalSkinWidth / 2.0f && hit.distance < HorizontalSkinWidth) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					_charMovement.AddForce(-dir, (1 - Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal))) * ((hit.distance - HorizontalSkinWidth / 2.0f)));
					var action = _actions.GetAction(playerActionType);
					action.Disable();
				}
			}

			if (collidersFound == 0) {
				var action = _actions.GetAction(playerActionType);
				action.Enable();
			}

		}

	}
}