using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Triggers {

	public class TFrontCart : MonoBehaviour {

		private Player _player;
		private DriveCartInteraction _cartInteraction;
		private bool _isChecked = false;

		public void Start() {
			_player = Player.GetInstance();
			_cartInteraction = new DriveCartInteraction(transform.parent.gameObject);
		}

		public void OnTriggerStay(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(WalkRunState)) {
				if (!_isChecked) {
					_player.Movement.MovementBehaviour.AddInteractionWithPriority(_cartInteraction);
					_isChecked = true;
				}
			}
			else {
				_isChecked = false;
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (_player.CurrentState.GetType() == typeof(WalkRunState)) { 
				_player.Movement.MovementBehaviour.RemoveInteraction(_cartInteraction);
			}
			_isChecked = false;
		}
	}
}
