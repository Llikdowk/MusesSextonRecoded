using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {
	public class TFrontCart : MonoBehaviour {

		private void SetUse() {
			Debug.Log("Use front car!");
		}

		public void Start() {
			Debug.Log("Trigger START");
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != "Player") return; // TODO use TAG manager 

			Debug.Log("ontriggerentered");
			Player.GetInstance().Actions.GetAction(ActionTag.Use)
				.StartBehaviour = SetUse;
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != "Player") return;

			Debug.Log("ontriggerexit");
		}
	}
}
