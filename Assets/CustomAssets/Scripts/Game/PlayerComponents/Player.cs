using UnityEngine;

namespace Game.PlayerComponents {
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {
		private static Player _instance;
		public static Player GetInstance() {return _instance; }

		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;
		public ActionManager Actions = new ActionManager();

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(this.gameObject);
				return;
			}

			_instance = this;
			Movement = GetComponent<CharacterMovement>();
			Controller = GetComponent<CharacterController>();

			Actions
				.AddAction(new Action(ActionTag.MoveForward, KeyCode.W))
				.AddAction(new Action(ActionTag.MoveLeft, KeyCode.A))
				.AddAction(new Action(ActionTag.MoveBack, KeyCode.S))
				.AddAction(new Action(ActionTag.MoveRight, KeyCode.D))
				.AddAction(new Action(ActionTag.Use, KeyCode.Mouse0));
		}
	}
}
