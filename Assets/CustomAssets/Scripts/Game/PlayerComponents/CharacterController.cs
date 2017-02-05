using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayerComponents {
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class CharacterController : MonoBehaviour {

		public Vector3 Gravity = Physics.gravity;
		public float GravityMultiplier = 1.0f;
		[Range(0, 0.5f)] public float SkinWidth = 0.15f;
		[Range(0, 1.0f)] public float SlopeInclinationAllowance = 0.1f;
		[Range(0, 1.0f)] public float StepAllowance = 0.0f;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private float _timeOnAir = 0.0f;
		private CharacterMovement _charMovement;

		private ActionManager _actions;

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
			int player = 1 << LayerMaskManager.GetInstance().GetLayer(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();
		}


		public void Update() {
			Debug.DrawRay(transform.position, _charMovement.WorldDir, Color.cyan);
			if (!IsGrounded) {
				_charMovement.AddForce(Gravity, _timeOnAir * Time.deltaTime);
			}
			Matrix4x4 M = _collider.transform.localToWorldMatrix;
			Vector3 capsuleHead = transform.position + M.MultiplyVector(_collider.center) +
			                      transform.up * (_collider.height / 2.0f - _collider.radius);
			Vector3 capsuleFeet = transform.position + M.MultiplyVector(_collider.center) -
			                      transform.up * (_collider.height / 2.0f - _collider.radius);


			CheckWalls(capsuleHead, capsuleFeet, transform.forward, ActionTag.MoveForward);
			CheckWalls(capsuleHead, capsuleFeet, -transform.forward, ActionTag.MoveBack);
			CheckWalls(capsuleHead, capsuleFeet, transform.right, ActionTag.MoveRight);
			CheckWalls(capsuleHead, capsuleFeet, -transform.right, ActionTag.MoveLeft);


			//CheckFloor
			RaycastHit[] floorHits = Physics.SphereCastAll(capsuleFeet, _collider.radius, -Vector3.up, SkinWidth,
				_layerMaskAllButPlayer);
			if (floorHits.Length == 0) IsGrounded = false;

			foreach (RaycastHit hit in floorHits) {
				// overlapping collision
				if (hit.point == Vector3.zero) {
					_charMovement.AddForce(hit.normal, _charMovement.StepMovement.magnitude + SkinWidth / 2.0f);
					IsGrounded = true;
				}
				else if (hit.distance > SkinWidth / 2.0f) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					_charMovement.AddForce(Vector3.down, hit.distance - SkinWidth/2.0f);
					IsGrounded = true;
				}
			}

			if (!IsGrounded) {
				_timeOnAir += Time.deltaTime;
			}
		}

		private void CheckWalls(Vector3 capsuleHead, Vector3 capsuleFeet, Vector3 dir, ActionTag actionType) {
			Vector3 stepOffset = transform.up * StepAllowance;
			RaycastHit[] wallHits = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, dir,
				4 * SkinWidth, _layerMaskAllButPlayer);
			foreach (RaycastHit hit in wallHits) {
				if (hit.point == Vector3.zero) {
					_charMovement.AddForce(hit.normal, _charMovement.StepMovement.magnitude + SkinWidth / 2.0f);
				}
				else if (Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal)) >= (1 - SlopeInclinationAllowance)) {
					continue;
				}
				else if (hit.distance > SkinWidth / 2.0f && hit.distance < SkinWidth) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					_charMovement.AddForce(-dir, (1 - Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal))) * ((hit.distance - SkinWidth / 2.0f)));
					Action action = _actions.GetAction(actionType);
					action.Disable();
				}
			}

			if (wallHits.Length == 0) {
				Action action = _actions.GetAction(actionType);
				action.Enable();
			}

		}

	}
}