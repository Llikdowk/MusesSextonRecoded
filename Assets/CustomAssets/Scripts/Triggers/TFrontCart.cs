using Game;
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
			if (other.tag != TagManager.Get(Tag.Player)) return; 

			Debug.Log("ontriggerentered");
			Player.GetInstance().Actions.GetAction(PlayerAction.Use)
				.StartBehaviour = SetUse;
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			Debug.Log("ontriggerexit");
		}
	}
}
