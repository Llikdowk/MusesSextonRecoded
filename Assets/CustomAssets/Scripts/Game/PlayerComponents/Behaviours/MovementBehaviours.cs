﻿using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayerComponents.Behaviours {

	public abstract class MovementBehaviour {
		protected MovementHandler _movement = new RawMovementHandler();
		protected Transform _transform;

		public Vector3 StepMovement { get { return _stepMovement; } }
		protected Vector3 _stepMovement;

		public Vector3 SelfMovement { get { return _movement.SelfMovement; } }
		public Vector3 WorldMovement { get { return _transform.localToWorldMatrix.MultiplyVector(_movement.SelfMovement); } }
		public Vector3 SelfDir { get { return _movement.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return _transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		protected MovementBehaviour(Transform transform) {
			_transform = transform;
		}

		public void AddForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public abstract void Step();
		public abstract void Clear();
	}


	public class NullMovementBehaviour : MovementBehaviour {
		public NullMovementBehaviour(Transform transform) : base(transform) {}
		public override void Step() {}
		public override void Clear() {}
	}


	public class WalkMovementBehaviour : MovementBehaviour {

		private readonly MovementConfig _walkConfig;
		private readonly MovementConfig _runConfig;
		private readonly Action<PlayerAction> _runAction;

		public WalkMovementBehaviour(Transform transform, SuperConfig config) : base(transform) {
			_walkConfig = config.WalkMovement;
			_runConfig = config.RunMovement;
			_movement = new SmoothMovementHandler(config.WalkRunAcceleration);
			_movement.SetMovement();
			_runAction = Player.GetInstance().Actions.GetAction(PlayerAction.Run);
		}

		public override void Step() {
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfDir;
			Vector3 dvelSelf = Vector3.zero;
			var config = _runAction.TimeActionActive > 0.0f ? _runConfig : _walkConfig;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? config.RightSpeed : config.LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? config.ForwardSpeed : config.BackwardSpeed);
			
			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;
		}

		public override void Clear() {}
	}


	public class CartMovementBehaviour : MovementBehaviour {

		private readonly CartMovementConfig _cartConfig;
		private readonly Transform _cartTransform;
		private readonly List<Collider> _disabledColliders = new List<Collider>();
		private int _layerMaskAllButPlayer;
		private readonly Action<PlayerAction> _moveBackAction;

		public CartMovementBehaviour(Transform transform, GameObject cart, SuperConfig config) : base(transform) {
			_cartConfig = config.DriveCartMovement;
			_movement = new SmoothMovementHandler(config.CartAcceleration);
			_movement.SetMovement();
			_cartTransform = cart.transform;
			foreach (Collider c in cart.GetComponentsInChildren<Collider>()) {
				if (!c.isTrigger) {
					c.enabled = false;
					_disabledColliders.Add(c);
				}
			}
			_layerMaskAllButPlayer = ~1 << LayerMaskManager.Get(Layer.Player);
			_moveBackAction = Player.GetInstance().Actions.GetAction(PlayerAction.MoveBack);
		}

		public override void Clear() {
			foreach (Collider c in _disabledColliders) {
				c.enabled = true;
			}
		}

		public override void Step() {

			Debug.Log(SelfMovement);
			Ray ray = new Ray(_cartTransform.position, -_cartTransform.forward);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.magenta);
			if (Physics.Raycast(ray, out hit, 10, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
					_moveBackAction.Disable();
			}
			else {
				_moveBackAction.Enable();
			}
			

			_cartTransform.position = Vector3.Lerp(_transform.position - _transform.forward * _cartConfig.DistanceToPlayer, 
				_cartTransform.position, _cartConfig.MovementLag);
			//_cartTransform.LookAt(_transform);
			_cartTransform.LookAt(
				Vector3.Lerp(_transform.position, _cartTransform.forward + _cartTransform.position, _cartConfig.LookLag), Vector3.up);
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.z += SelfMovement.z * (SelfMovement.z > 0 ? _cartConfig.ForwardSpeed : 1);
			
			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;
		}
		
	}

}