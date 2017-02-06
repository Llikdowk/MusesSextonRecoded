using System;
using Game.PlayerComponents;
using UnityEngine;

namespace Game.PlayerComponents {

	public class InputController : MonoBehaviour {

		[Serializable]
		public struct InputAction {
			 public PlayerAction Action;
			 public KeyCode[] Keys;
		 }

		public InputAction[] InputConfig;
		private ActionManager<PlayerAction> _actions;


		public void Start() {
			_actions = Player.GetInstance().Actions;

			if (InputConfig.Length == 0) {
				InputConfig = new InputAction[Enum.GetValues(typeof(PlayerAction)).Length];

				InputConfig[0].Action = PlayerAction.MoveForward;
				InputConfig[0].Keys = new[] {KeyCode.W};
				InputConfig[1].Action = PlayerAction.MoveLeft;
				InputConfig[1].Keys = new[] {KeyCode.A};
				InputConfig[2].Action = PlayerAction.MoveBack;
				InputConfig[2].Keys = new[] {KeyCode.S};
				InputConfig[3].Action = PlayerAction.MoveRight;
				InputConfig[3].Keys = new[] {KeyCode.D};

				InputConfig[4].Action = PlayerAction.Use;
				InputConfig[4].Keys = new[] {KeyCode.E, KeyCode.Mouse0};
				InputConfig[5].Action = PlayerAction.Run;
				InputConfig[5].Keys = new[] {KeyCode.LeftShift};
			}

			foreach (InputAction inputAction in InputConfig) {
				var action = _actions.GetAction(inputAction.Action);
				foreach (KeyCode k in inputAction.Keys) {
					action.AddKey(k);
				}
			}

		}

		public void Update() { // Executed before any other script Update (check ScriptOrder)

			foreach (Action<PlayerAction> action in _actions.Actions) {
				foreach (KeyCode k in action.Keys) {
					if (UnityEngine.Input.GetKeyDown(k)) {
						action.StartBehaviour();
						action.ActivationTime = Time.time;
						action.FinalizationTime = Time.time;
						break;
					}

					if (UnityEngine.Input.GetKey(k)) {
						action.WhileBehaviour();
						break;
					}

					if (UnityEngine.Input.GetKeyUp(k)) {
						action.FinishBehaviour();
						action.ActivationTime = Time.time;
						action.FinalizationTime = Time.time;
						break;
					}

					action.NotPressedBehaviour();
				}
			}

		}

	}


}
