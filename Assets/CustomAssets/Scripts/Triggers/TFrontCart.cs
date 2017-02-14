using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using MiscComponents;
using UnityEngine;

namespace Triggers {

	public class TFrontCart : MonoBehaviour {

		private Player _player;

		public void Start() {
			_player = Player.GetInstance();
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(WalkRunState)) {
				_player.Movement.MovementBehaviour.Interaction = new DriveCartInteraction(transform.parent.gameObject);
			}
		}
	}
}
