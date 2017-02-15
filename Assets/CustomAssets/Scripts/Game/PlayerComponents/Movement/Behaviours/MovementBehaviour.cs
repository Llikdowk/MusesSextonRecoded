using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public abstract class MovementBehaviour {
		protected MovementHandler _movement = new NullMovementHandler();
		protected Transform _transform;

		public Vector3 StepMovement { get { return _stepMovement; } }
		protected Vector3 _stepMovement;

		public bool CanInteract = true;

		public Interaction CurrentInteraction {
			get { return _currentInteraction; }
			set {
				_currentInteraction = value;
			}	
		}

		private Interaction _currentInteraction = new EmptyInteraction();
		public readonly C5.ArrayList<Interaction> _availableInteractions = new C5.ArrayList<Interaction>();

		public Vector3 SelfMovement { get { return _movement.SelfMovement; } }
		public Vector3 WorldMovement { get { return _transform.localToWorldMatrix.MultiplyVector(_movement.SelfMovement); } }
		public Vector3 SelfDir { get { return _movement.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return _transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		protected MovementBehaviour(Transform transform) {
			Player.GetInstance().Actions.ResetAllActions();
			_transform = transform;
		}


		public abstract void Step();
		public abstract void OnDestroy();
	}

}