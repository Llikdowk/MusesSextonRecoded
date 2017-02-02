using System.Security.AccessControl;
using Game.Actions;
using UnityEngine;

namespace CustomInput {


	public class Input : MonoBehaviour {

		public string outside;
		private ActionManager _actionMng;

		public void Awake() {
			_actionMng = new ActionManager();
			_actionMng.Actions[0].WhileBehaviour = () => { Debug.Log(outside); };// Action.nop; // TODO: add action id in actionmng
		}

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
