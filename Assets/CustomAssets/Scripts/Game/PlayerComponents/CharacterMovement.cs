using Game.PlayerComponents.Movement.Behaviours;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Game.PlayerComponents {

	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {


		public MovementBehaviour MovementBehaviour {
			set {
				_movementBehaviour.CurrentInteraction = new EmptyInteraction();
				_movementBehaviour.OnDestroy();
				_movementBehaviour = value;
				_movementBehaviour.CanInteract = _canInteract;
			}
			get {
				return _movementBehaviour;
			}
		}

		private MovementBehaviour _movementBehaviour;

		public Vector3 StepMovement {
			get { return _movementBehaviour.StepMovement; }
		}
		public Vector3 SelfMovement { get { return _movementBehaviour.SelfMovement; } }
		public Vector3 WorldMovement { get { return _movementBehaviour.WorldMovement; } }
		public Vector3 SelfDir { get { return _movementBehaviour.SelfDir; } }
		public Vector3 WorldDir { get { return _movementBehaviour.WorldDir; } }
		private bool _canInteract = true;

		public void Awake() {
			_movementBehaviour = new NullMovementBehaviour(transform);
		}

		public void Update() {
			_movementBehaviour.Step();
		}

	}
}
