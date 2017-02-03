using UnityEngine;

namespace Game.CustomInput {

	public class InputController : MonoBehaviour {

		private readonly ActionManager _actionMng = ActionManager.GetInstance();

		public void Update() {

			foreach (Action action in _actionMng.Actions) {
				foreach (KeyCode k in action.Keys) {
					if (UnityEngine.Input.GetKeyDown(k)) {
						action.StartBehaviour();
						break;
					}

					if (UnityEngine.Input.GetKey(k)) {
						action.WhileBehaviour();
						break;
					}

					if (UnityEngine.Input.GetKeyUp(k)) {
						action.FinishBehaviour();
						break;
					}
				}
			}

		}

	}


}
