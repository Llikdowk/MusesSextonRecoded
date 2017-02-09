﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {
	public class CartMovementBehaviour : MovementBehaviour {

		private readonly CartMovementConfig _cartConfig;
		private readonly Transform _cartTransform;
		private readonly List<Collider> _disabledColliders = new List<Collider>();
		private readonly int _layerMaskAllButPlayer;
		private readonly Action<PlayerAction> _moveBackAction;

		public CartMovementBehaviour(Transform transform, GameObject cart, SuperConfig config) : base(transform) {
			Player.GetInstance().Look.Config = config.DriveCartLook;

			config.DriveCartLook.FixedForward = transform;
			_cartConfig = config.DriveCartMovement;
			_movement = new SmoothMovementHandler(config.CartAcceleration).SetMovement();
			_cartTransform = cart.transform;
			foreach (Collider c in cart.GetComponentsInChildren<Collider>()) {
				if (!c.isTrigger) {
					c.enabled = false;
					_disabledColliders.Add(c);
				}
			}
			_layerMaskAllButPlayer = ~ (1 << LayerMaskManager.Get(Layer.Player));
			_moveBackAction = Player.GetInstance().Actions.GetAction(PlayerAction.MoveBack);

			Utils.Animation.SlerpForward(transform, cart.transform, _cartConfig.GoInsideTimeSeconds, () => {
				Player.GetInstance().Actions.GetAction(PlayerAction.Use).StartBehaviour = () => {
					Player.GetInstance().CurrentState = new WalkRunState();
				};
			});

		}

		public override void ResetModifiedState() {
			foreach (Collider c in _disabledColliders) {
				c.enabled = true;
			}
		}

		public override void Step() {
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Ray ray = new Ray(_cartTransform.position, -_cartTransform.forward);
			RaycastHit hit;
			float cartLength = 7.5f; // TODO parametrice this distance (maybe using a bounding box?)
			Debug.DrawRay(ray.origin, ray.direction * cartLength, Color.magenta);
			if (Physics.Raycast(ray, out hit, cartLength, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				_moveBackAction.Disable();
			}
			else {
				_moveBackAction.Enable();
			}


			Vector3 newForward = Vector3.Slerp(_cartTransform.forward, _transform.forward,
				(1 - _cartConfig.LookLag) * Mathf.Abs(SelfMovement.z));
			ray = new Ray(_cartTransform.position, Vector3.down);
			if (Physics.Raycast(ray, out hit, 10.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				Vector3 cross = Vector3.Cross(_cartTransform.right, hit.normal);
				newForward += Vector3.RotateTowards(_cartTransform.forward, cross, Time.deltaTime * _cartConfig.VerticalRotationStep,
					0.0f);
			}

			_cartTransform.rotation = Quaternion.LookRotation(newForward);


			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.z += SelfMovement.z * (SelfMovement.z > 0 ? _cartConfig.ForwardSpeed : _cartConfig.BackwardSpeed);
			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;

			_cartTransform.position = Vector3.Lerp(
				_transform.position - _cartTransform.forward * _cartConfig.DistanceToPlayer,
				_cartTransform.position,
				_cartConfig.MovementLag);



		}

	}
}