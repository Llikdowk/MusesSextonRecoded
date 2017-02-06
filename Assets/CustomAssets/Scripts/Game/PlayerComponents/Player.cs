using UnityEngine;


namespace Game.PlayerComponents {

	public enum PlayerAction {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack, Run
	}

	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {
		private static Player _instance;
		public static Player GetInstance() {return _instance; }

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
		}
	}
}
