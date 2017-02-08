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
		public WalkState() {
			Movement.SetWalkBehaviour();
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState(GameObject cart) {
			Movement.SetCartBehaviour(cart);
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
		[HideInInspector] public Look Look;
		[HideInInspector] public Camera Camera { get; private set; }

		public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(this.gameObject);
				return;
			}

			_instance = this;
			Movement = GetComponent<CharacterMovement>();
			Controller = GetComponent<CharacterController>();
			Look = GetComponent<Look>();
			Camera = GetComponentInChildren<Camera>();
		}

		public void Start() {
			CurrentState = new WalkState();
		}
	}
}
