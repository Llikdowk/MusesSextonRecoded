using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Triggers {

	public class TFrontCart : MonoBehaviour {

		private Player _player;
		private DriveCartInteraction _cartInteraction;

		public void Start() {
			_player = Player.GetInstance();
			_cartInteraction = new DriveCartInteraction(transform.parent.gameObject);
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(WalkRunState) || _player.CurrentState.GetType() == typeof(WalkRunStateFromCart)) {
				_player.Movement.MovementBehaviour.AvailableInteractions.Insert(0, _cartInteraction);
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (_player.CurrentState.GetType() == typeof(WalkRunState) || _player.CurrentState.GetType() == typeof(WalkRunStateFromCart)) { 
				_player.Movement.MovementBehaviour.AvailableInteractions.Remove(_cartInteraction);
			}
		}
	}
}
