using Game.PlayerComponents;
using UnityEngine;

namespace Game.CustomInput {

	public class InputController : MonoBehaviour {

		private ActionManager _actionMng;

		public void Start() {
			_actionMng = Player.GetInstance().Actions;
		}

		public void Update() { // Executed before anything else (check ScriptOrder)

			foreach (Action action in _actionMng.Actions) {
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
