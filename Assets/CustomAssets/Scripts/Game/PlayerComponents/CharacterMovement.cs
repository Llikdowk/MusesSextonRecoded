using Game.PlayerComponents.Movement.Behaviours;
using UnityEngine;

namespace Game.PlayerComponents {

	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {


		public MovementBehaviour MovementBehaviour {
			set {
				_movementBehaviour.ResetModifiedState();
				_movementBehaviour = value;
				_movementBehaviour.CanInteract = _canInteract;
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

			//CheckMovement();
			_movementBehaviour.Step();
		}

		private void CheckMovement() {
			CharacterController c = Player.GetInstance().Controller; // TODO: make member of this class and remove from player
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;
			uint collisions = c.GetCollisions();
			Debug.Log(collisions);
			uint mask = (uint) CollisionMask.None;

			mask = (uint) CollisionMask.Forward;
			if (collisions * mask > 0) {
				actions.GetAction(PlayerAction.MoveForward).Disable();
			}
			else {
				actions.GetAction(PlayerAction.MoveForward).Enable();
			}


			mask = (uint) CollisionMask.Back;
			if (collisions * mask > 0) {
				actions.GetAction(PlayerAction.MoveBack).Disable();
			}
			else {
				actions.GetAction(PlayerAction.MoveBack).Enable();
			}


			mask = (uint) CollisionMask.Right;
			if (collisions * mask > 0) {
				actions.GetAction(PlayerAction.MoveRight).Disable();
			}
			else {
				actions.GetAction(PlayerAction.MoveRight).Enable();
			}


			mask = (uint) CollisionMask.Left;
			if (collisions * mask > 0) {
				actions.GetAction(PlayerAction.MoveLeft).Disable();
			}
			else {
				actions.GetAction(PlayerAction.MoveLeft).Enable();
			}
			
		}

		//TODO deprecate
		public void AddForce(Vector3 dir, float force) {
			_movementBehaviour.AddForce(dir, force);
		}

		public void CheckInternalInteraction(bool interaction) {
			_canInteract = interaction;
			_movementBehaviour.CanInteract = interaction;
		}

	}
}
