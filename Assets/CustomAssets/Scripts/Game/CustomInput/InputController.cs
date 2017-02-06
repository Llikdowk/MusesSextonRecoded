using Game.PlayerComponents;
using UnityEngine;

namespace Game.CustomInput {

	public class InputController : MonoBehaviour {

		private ActionManager<PlayerAction> _actions;

		public void Start() {
			_actions = Player.GetInstance().Actions;

			_actions.GetAction(PlayerAction.MoveForward).AddKey(KeyCode.W);
			_actions.GetAction(PlayerAction.MoveLeft).AddKey(KeyCode.A);
			_actions.GetAction(PlayerAction.MoveBack).AddKey(KeyCode.S);
			_actions.GetAction(PlayerAction.MoveRight).AddKey(KeyCode.D);
			_actions.GetAction(PlayerAction.Use).AddKey(KeyCode.Mouse0).AddKey(KeyCode.E);
			_actions.GetAction(PlayerAction.Run).AddKey(KeyCode.LeftShift);

		}

		public void Update() { // Executed before anything else (check ScriptOrder)

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
