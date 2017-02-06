using UnityEngine;


namespace Game.PlayerComponents {

	public enum PlayerAction {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack, Run
	}

	/*
	public enum PlayerState {
		Walk, DriveCart, Dig, Poem, DragCoffin
	}
	*/

	public abstract class PlayerState {
		protected PlayerState() {
			this.Movement = Player.GetInstance().Movement;
		}

		protected CharacterMovement Movement;
	}

	public class WalkState : PlayerState {
		public WalkState() { Start(); }
		public void Start() {
			Debug.Log("WalkState start");
			Movement.BackwardSpeed = 5.0f;
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState() { Start(); }

		public void Start() {
			Debug.Log("DriveCart start");
			Movement.BackwardSpeed = 0.0f;
		}
	}

	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {
		private static Player _instance;
		public static Player GetInstance() {return _instance; }
		public PlayerState CurrentState;

		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;
		public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(this.gameObject);
				return;
			}

			_instance = this;
			Movement = GetComponent<CharacterMovement>();
			Controller = GetComponent<CharacterController>();
			CurrentState = new WalkState();
		}
	}
}
