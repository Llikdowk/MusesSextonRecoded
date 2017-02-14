using System.Reflection;
using System.Security.Cryptography;
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
			_movement = Player.GetInstance().Movement;
			_transform = Player.GetInstance().transform;
			_config = Player.GetInstance().Config;
		}

		public void CheckInternalInteraction(bool interaction) {
			_movement.CheckInternalInteraction(interaction);
		}

		protected CharacterMovement _movement;
		protected Transform _transform;
		protected SuperConfig _config;
	}

	public class WalkRunState : PlayerState {
		public WalkRunState() {
			_movement.MovementBehaviour = new WalkRunMovementBehaviour(_transform, _config);
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState(GameObject cart) {
			_movement.MovementBehaviour = new CartMovementBehaviour(_transform, cart, _config);
		}
	}

	public class DragCoffinState : PlayerState {
		public DragCoffinState(GameObject coffin) {
			_movement.MovementBehaviour = new DragCoffinBehaviour(_transform, coffin, _config);
		}
	}

	public class DigDownState : PlayerState {
		public DigDownState(GameObject ground) {
			_movement.MovementBehaviour = new DigBehaviour(_transform, ground);
		}
	}


	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {

		public PlayerState CurrentState;
		public SuperConfig Config;

		// TODO: Movement should not be accessible from everywhere, its changes should be limited in PlayerState classes
		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;
		[HideInInspector] public Look Look;
		[HideInInspector] public Camera Camera { get; private set; }
		[HideInInspector] public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();
		private CapsuleCollider _collider;


		private static Player _instance = null;

		public static Player GetInstance() {
			if (_instance == null) {
				Debug.LogError("Player singleton called but it is not yet built");
			}
			return _instance;
		}

		public void Awake() {
			if (_instance == null) {
				_instance = this;
				DontDestroyOnLoad(gameObject);

				Movement = GetComponent<CharacterMovement>();
				Controller = GetComponent<CharacterController>();
				Look = GetComponent<Look>();
				Camera = GetComponentInChildren<Camera>();
				_collider = GetComponent<CapsuleCollider>();
			}
			else {
				Debug.LogWarning("Player singleton instance destroyed!");
				Destroy(gameObject);
			}
		}

		public void Start() {
			CurrentState = new WalkRunState();
		}

		public void MoveImmediatlyTo(Vector3 position) {
			transform.position = position + Vector3.up * _collider.height/2.0f;
		}
	}
}
