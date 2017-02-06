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
	}

	public class WalkMovementBehaviour : MovementBehaviour {

		private readonly MovementConfig _walkConfig;
		private readonly MovementConfig _runConfig;
		private readonly Action<PlayerAction> _runAction;

		public WalkMovementBehaviour(Transform transform, SuperConfig config) : base(transform) {
			_walkConfig = config.WalkMovement;
			_runConfig = config.RunMovement;
			_movement = new SmoothMovementHandler(config.SmoothAcceleration);
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

	}

	public class NullMovementBehaviour : MovementBehaviour {
		public NullMovementBehaviour(Transform transform) : base(transform) {}
		public override void Step() {}
	}

}