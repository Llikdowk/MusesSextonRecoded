
using Game;
using Game.PlayerComponents;
using UI;
using UnityEngine;

namespace Triggers {
	public class TEndGame : MonoBehaviour {
		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (GameState.HasEnded) {
				UIGame.GetInstance().FadeToBlack(2.5f, () => {
					Application.OpenURL("https://twitter.com/intent/tweet?button_hashtag=MusesSexton&text="
					                    + Player.GetInstance().GetPlayerTombVerse()
					                    + "%0D" + Player.GetInstance().GetPlayerTombVerse()
					                    + "%0D" + Player.GetInstance().GetPlayerTombVerse()
					                    + "%0D");
					Application.Quit();
				});
			}
		}
	}
}
