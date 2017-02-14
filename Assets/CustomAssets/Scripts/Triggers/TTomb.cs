
using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {
	public class TTomb : MonoBehaviour {
		private Player _player;

		public void Awake() {
			_player = Player.GetInstance();
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (_player.CurrentState.GetType() == typeof(DragCoffinState)) {
				Debug.Log("player has entered with a coffin!");
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			Debug.Log("player has exited the tomb interaction zone!");
		}

	}
}
