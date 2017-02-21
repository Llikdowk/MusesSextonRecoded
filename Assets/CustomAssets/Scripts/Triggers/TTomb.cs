
using Game;
using Game.Entities;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Triggers {
	public class TTomb : MonoBehaviour {
		private Player _player;
		private bool _isChecked = false;
		private TombComponent _tombComponent;
		private SendCoffinToTombInteraction _sendCoffinInteraction;

		public void Awake() {
			_player = Player.GetInstance();
		}

		public void Init(TombComponent tombComponent) {
			_tombComponent = tombComponent;
		}

		public void OnTriggerStay(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(DragCoffinState)) {
				if (!_isChecked) {
					_sendCoffinInteraction = new SendCoffinToTombInteraction(_tombComponent, ((DragCoffinState) _player.CurrentState).Coffin);
					Player.GetInstance().Movement.MovementBehaviour.AddInteractionWithPriority(_sendCoffinInteraction);
					_isChecked = true;
				}
			}
			else {
				_isChecked = false;
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			_isChecked = false;
			Player.GetInstance().Movement.MovementBehaviour.RemoveInteraction(_sendCoffinInteraction);
		}

	}
}
