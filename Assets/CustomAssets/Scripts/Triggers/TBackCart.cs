using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Triggers {

	public class TBackCart : MonoBehaviour {

		private Player _player;
		private SaveCoffinInCartInteraction _cartInteraction;
		private bool _isChecked = false;
		private Transform _coffinSet;

		public void Start() {
			_player = Player.GetInstance();
			foreach (Transform t in transform.parent.GetComponentInChildren<Transform>()) {
				if (t.gameObject.name == "CoffinSet") {
					_coffinSet = t;
				}
			}
			if (!_coffinSet) {
				DebugMsg.GameObjectNotFound(Debug.LogError, "CoffinSet");
			}
		}

		public void OnTriggerStay(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			if (_player.CurrentState.GetType() == typeof(DragCoffinState)) {
				if (!_isChecked) {
					Transform slot = null;
					foreach (Transform t in _coffinSet.transform) {
						if (t.childCount == 0) {
							slot = t;
							break;
						}
					}
					if (slot) {
						_cartInteraction = new SaveCoffinInCartInteraction(((DragCoffinState) _player.CurrentState).Coffin, slot);
						_player.Movement.MovementBehaviour.AddInteractionWithPriority(_cartInteraction);
					}
					_isChecked = true;
				}
			}
			else {
				_isChecked = false;
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (_player.CurrentState.GetType() == typeof(DragCoffinState)) {
				if (_cartInteraction != null) {
					_player.Movement.MovementBehaviour.RemoveInteraction(_cartInteraction);
				}
			}
			_isChecked = false;
		}
	}
}
