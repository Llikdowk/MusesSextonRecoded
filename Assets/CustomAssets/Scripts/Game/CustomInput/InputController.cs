using UnityEngine;

namespace Game.CustomInput {

	public class InputController : MonoBehaviour {

		private readonly ActionManager _actionMng = ActionManager.GetInstance();

		public void Update() { // Executed before anything else (check ScriptOrder)

			foreach (Action action in _actionMng.Actions) {
				foreach (KeyCode k in action.Keys) {
					if (UnityEngine.Input.GetKeyDown(k)) {
						action.StartBehaviour();
						action.TimeActionActive = 0.0f;
						break;
					}

					if (UnityEngine.Input.GetKey(k)) {
						action.WhileBehaviour();
						action.TimeActionActive += Time.deltaTime;
						break;
					}

					if (UnityEngine.Input.GetKeyUp(k)) {
						action.FinishBehaviour();
						action.TimeActionActive = 0.0f;
						break;
					}
				}
			}

		}

	}


}
