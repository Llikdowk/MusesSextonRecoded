using Game.PlayerComponents.Movement;
using Game.PlayerComponents.Movement.Behaviours;
using UnityEngine;

namespace Game.PlayerComponents {

	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		public SuperConfig Config;

		public MovementBehaviour MovementBehaviour {
			get { return _movementBehaviour; }
			set {
				_movementBehaviour.Clear();
				_movementBehaviour = value;
			}
		}

		private MovementBehaviour _movementBehaviour;

		public Vector3 StepMovement {
			get { return _movementBehaviour.StepMovement; }
		}
		public Vector3 SelfMovement { get { return MovementBehaviour.SelfMovement; } }
		public Vector3 WorldMovement { get { return MovementBehaviour.WorldMovement; } }
		public Vector3 SelfDir { get { return MovementBehaviour.SelfDir; } }
		public Vector3 WorldDir { get { return MovementBehaviour.WorldDir; } }

		public void Awake() {
			_movementBehaviour = new NullMovementBehaviour(transform);
		}

		public void Update() {
			_movementBehaviour.Step();
		}

		public void AddForce(Vector3 dir, float force) {
			_movementBehaviour.AddForce(dir, force);
		}


		public void SetNullBehaviour() {
			Debug.Log("set NULL movBehaviour");
			MovementBehaviour = new NullMovementBehaviour(transform);
		}
		public void SetWalkRunBehaviour() {
			Debug.Log("set WALK movBehaviour");
			MovementBehaviour = new WalkRunMovementBehaviour(transform, Config);
		}
		public void SetDragCoffinBehaviour() {
			Debug.Log("set DRAGCOFFIN movBehaviour");
			MovementBehaviour = new DragCoffinBehaviour(transform, Config);
		}
		public void SetCartBehaviour(GameObject cart) {
			Debug.Log("set CART movBehaviour");
			MovementBehaviour = new CartMovementBehaviour(transform, cart, Config);
		}
	}
}
