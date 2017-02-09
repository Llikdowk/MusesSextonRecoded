using System.Reflection;
using Game.PlayerComponents.Movement;
using Game.PlayerComponents.Movement.Behaviours;
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
			Movement = Player.GetInstance().Movement;
			Transform = Player.GetInstance().transform;
			Config = Player.GetInstance().Config;
		}

		protected CharacterMovement Movement;
		protected Transform Transform;
		protected SuperConfig Config;
	}

	public class WalkRunState : PlayerState {
		public WalkRunState() {
			Movement.MovementBehaviour = new WalkRunMovementBehaviour(Transform, Config);
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState(GameObject cart) {
			Movement.MovementBehaviour = new CartMovementBehaviour(Transform, cart, Config);
		}
	}

	public class DragCoffinState : PlayerState {
		public DragCoffinState(GameObject coffin) {
			Movement.MovementBehaviour = new DragCoffinBehaviour(Transform, coffin, Config);
		}
	}

	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {

		public PlayerState CurrentState;
		public SuperConfig Config;

		public MovementBehaviour behaviour;
		
		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;
		[HideInInspector] public Look Look;
		[HideInInspector] public Camera Camera { get; private set; }
		[HideInInspector] public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();


		private static Player _instance;
		public static Player GetInstance() {return _instance; }

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(this.gameObject);
				return;
			}

			_instance = this;
			Movement = gameObject.GetOrAddComponent<CharacterMovement>();
			//behaviour = Movement.MovementBehaviour;
			Controller = GetComponent<CharacterController>();
			Look = GetComponent<Look>();
			Camera = GetComponentInChildren<Camera>();
		}

		public void Start() {
			CurrentState = new WalkRunState();
		}
	}
}
