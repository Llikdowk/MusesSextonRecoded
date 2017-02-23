using Game;
using Game.PlayerComponents;
using UnityEngine;
using Utils;

namespace Triggers {
	public class TFinalArea : MonoBehaviour {
		private Transform _finalTomb;

		public void Awake() {
			_finalTomb = GameObject.Find("_finalTombstone").transform;
		}

		public void OnTriggerEnter(Collider other) {
			if (other.gameObject.tag != TagManager.Get(Tag.Player)) return;

			Player player = Player.GetInstance();
			AnimationUtils.LookTowardsHorizontal(player.transform, (_finalTomb.position - player.transform.position).normalized, 0.5f);
			AnimationUtils.LookTowardsVertical(player.MainCamera.transform, _finalTomb.position, 0.5f,
				()=>player.CurrentState = new PlayerPoemState()
			);
		}
	}
}
