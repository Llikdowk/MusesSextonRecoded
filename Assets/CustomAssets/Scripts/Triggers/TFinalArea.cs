using Game;
using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {
	public class TFinalArea : MonoBehaviour {

		public void OnTriggerEnter(Collider other) {
			if (other.gameObject.tag != TagManager.Get(Tag.Player)) return;

			Player.GetInstance().CurrentState = new PlayerPoemState();
		}
	}
}
