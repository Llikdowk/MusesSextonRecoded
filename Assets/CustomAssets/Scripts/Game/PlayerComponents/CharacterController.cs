using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Game.PlayerComponents {
	public enum CollisionMask {
		None = 0x000, Forward = 0x0001, Back = 0x0010, Right = 0x0100, Left = 0x1000
	}

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
		[Range(-1, 0)] public float NormalTolerance = -0.65f;
		public bool InteractsWithTriggers = true;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private float _timeOnAir = 0.0f;
		private CharacterMovement _charMovement;

		private bool _isGrounded = false;

		private const int MaxSimultaneousColliders = 32;
		private RaycastHit[] _colliderHits = new RaycastHit[MaxSimultaneousColliders];
		private List<Collider> _currentTriggers = new List<Collider>(); 
		private List<Collider> _stayTriggers = new List<Collider>();
		private uint _collisions;

		public void Start() {
			int player = 1 << LayerMaskManager.Get(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();

		}

		private int ProcessTriggers(ref RaycastHit[] hits, ref int hitsLength) {
			int triggerCount = 0;
			_currentTriggers.Clear();

			for (int i = hitsLength-1; i >= 0; --i) {
				if (hits[i].collider.isTrigger) {
					if (hits[i].distance == 0) {
						//_currentTriggers[triggerCount++] = hits[i].collider;
						_currentTriggers.Add(hits[i].collider);
						++triggerCount;
					}
					for (int j = i; j < hitsLength - 1; ++j) {
						hits[i] = hits[i + 1];
					}
					--hitsLength;
				}
			}

			if (InteractsWithTriggers) {
				foreach (Collider collider in _currentTriggers) {
					if (_stayTriggers.Contains(collider)) {
						collider.SendMessage("OnTriggerStay", _collider, SendMessageOptions.DontRequireReceiver);
					}
					else {
						collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
						_stayTriggers.Add(collider);
					}
				}

				for (int i = _stayTriggers.Count - 1; i >= 0; --i) {
					Collider collider = _stayTriggers[i];
					if (!_currentTriggers.Contains(collider)) {
						collider.SendMessage("OnTriggerExit", _collider, SendMessageOptions.DontRequireReceiver);
						_stayTriggers.Remove(collider);
					}
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
							while (j <= i + 1) {
								aux = hits[j];
								hits[j] = nextItem;
								nextItem = aux;
								++j;
							}
						}
					}
				}
			}
		}


		private Vector3 _lastPosition;
		public Vector3 WorldMovementProcessed = Vector3.zero; // TODO: encapsulate this
		public void Update() {
			float speed = (transform.position - _lastPosition).magnitude;
			_lastPosition = transform.position;
			Debug.DrawRay(transform.position, _charMovement.WorldDir, Color.cyan);
			Vector3 gravityForce = Vector3.zero;
			if (!_isGrounded) {
				gravityForce = Gravity * _timeOnAir * Time.deltaTime; // TODO: move to characterMovement?
				_timeOnAir += Time.deltaTime;
			}
			Matrix4x4 M = _collider.transform.localToWorldMatrix; //TODO: use transform.transformpoint instead
			Vector3 capsuleHead = transform.position + M.MultiplyVector(_collider.center) +
			                      transform.up * (_collider.height / 2.0f - _collider.radius); 
			Vector3 capsuleFeet = transform.position + M.MultiplyVector(_collider.center) -
			                      transform.up * (_collider.height / 2.0f - _collider.radius);

			
			// Vertical Collision
			int hitsCount = Physics.CapsuleCastNonAlloc(capsuleFeet, capsuleHead, _collider.radius, Vector3.down,
				_colliderHits, GrounderDistance, _layerMaskAllButPlayer, QueryTriggerInteraction.Collide);
			int triggerCount = ProcessTriggers(ref _colliderHits, ref hitsCount);
			

			if (hitsCount > 0) {
				Sort(ref _colliderHits, 0, hitsCount);
				RaycastHit nearHit = _colliderHits[0];
				if (nearHit.point == Vector3.zero) {
					const float pushBack = 10.0f;
					if (Vector3.Dot(nearHit.normal, Vector3.up) >= 1-SlopeInclinationAllowance) { // SlopeLimit!
						transform.position = transform.position + -gravityForce * Time.deltaTime * pushBack +
						                     nearHit.normal * VerticalSkinWidth / 2.0f; // hit.normal here is -ray.direction
					}
				}
				else if (nearHit.distance > VerticalSkinWidth) {
					Debug.DrawRay(transform.position, nearHit.point - transform.position, Color.magenta);
					transform.position += Vector3.down * (nearHit.distance - VerticalSkinWidth / 2.0f);
				}
				_isGrounded = true;
				_timeOnAir = 0.0f;
			}
			else {
				_isGrounded = false;
				transform.position += gravityForce;
			}

			// Horizontal Collisions
			WorldMovementProcessed = _charMovement.WorldMovement;
			Vector3 stepOffset = transform.up * StepAllowance;
			
			hitsCount = Physics.CapsuleCastNonAlloc(capsuleHead, capsuleFeet + stepOffset, _collider.radius, _charMovement.WorldDir,
				_colliderHits, HorizontalSkinWidth + speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);

			_collisions = (uint) CollisionMask.None;
			for (int i = 0; i < hitsCount; ++i) {
				RaycastHit hit = _colliderHits[i];
				Debug.DrawRay(hit.point, hit.normal, Color.magenta);
				WorldMovementProcessed = Vector3.ProjectOnPlane(WorldMovementProcessed, hit.normal);
				if (hit.point == Vector3.zero) {
					if (Vector3.Dot(hit.normal, Vector3.up) < 1 - SlopeInclinationAllowance) {
						transform.position += hit.normal * (HorizontalSkinWidth / 2.0f); // hit.normal <=> -ray.distance
					}
				}
				else {
					if (hit.distance < HorizontalSkinWidth) { // TODO: check connection with CharacterMovement, remove?
						float dot = Vector3.Dot(transform.forward, hit.normal);
						if (dot < NormalTolerance) {
							_collisions |= (uint) CollisionMask.Forward;
						}
						dot = Vector3.Dot(-transform.forward, hit.normal);
						if (dot < NormalTolerance) {
							_collisions |= (uint) CollisionMask.Back;
						}
						dot = Vector3.Dot(transform.right, hit.normal);
						if (dot < NormalTolerance) {
							_collisions |= (uint) CollisionMask.Right;
						}
						dot = Vector3.Dot(-transform.right, hit.normal);
						if (dot < NormalTolerance) {
							_collisions |= (uint) CollisionMask.Left;
						}
					}
				}
			}
		}
		


		public uint GetCollisions() {
			return _collisions;
		}

	}
}