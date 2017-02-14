using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public abstract class MovementBehaviour {
		protected MovementHandler _movement = new NullMovementHandler();
		protected Transform _transform;

		public Vector3 StepMovement { get { return _stepMovement; } }
		protected Vector3 _stepMovement;

		public bool CanInteract = true;

		public Interaction Interaction {
			get { return _interaction; }
			set {
				_interaction.HideFeedback();
				_interaction = value;
			}	
		}

		private Interaction _interaction = new EmptyInteraction();


		public Vector3 SelfMovement { get { return _movement.SelfMovement; } }
		public Vector3 WorldMovement { get { return _transform.localToWorldMatrix.MultiplyVector(_movement.SelfMovement); } }
		public Vector3 SelfDir { get { return _movement.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return _transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		protected MovementBehaviour(Transform transform) {
			Player.GetInstance().Actions.ResetAllActions();
			_transform = transform;
		}


		public void AddForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public abstract void Step();
		public abstract void OnDestroy();
	}

}