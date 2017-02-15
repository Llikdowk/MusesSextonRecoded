
using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Triggers {
	public class TTomb : MonoBehaviour {
		private Player _player;
		private bool _isChecked = false;
		private GameObject _ground;

		public void Init(GameObject ground) {
			_ground = ground;
		}

		public void Awake() {
			_player = Player.GetInstance();
		}


		public void OnTriggerStay(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(DragCoffinState)) {
				if (!_isChecked) {
					Player.GetInstance().Movement.MovementBehaviour.AddInteractionWithPriority(
						new SendCoffinToTombInteraction(gameObject, _ground, ((DragCoffinState) _player.CurrentState).Coffin));
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
		}

	}
}
