using Game;
using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {

	using Action = Action<PlayerAction>;

	public class TFrontCart : MonoBehaviour {

		private Player _player;
		private GameObject _model;

		public void Start() {
			_player = Player.GetInstance();
			foreach (Transform t in transform.parent.GetComponentInChildren<Transform>()) {
				if (t.gameObject.name == "Model") {
					_model = t.gameObject;
					break;
				}
			}
		}

		private void ToggleWalkDrive(GameObject cart) {
			PlayerState currentState = _player.CurrentState;

			if (currentState.GetType() == typeof(WalkRunState)) {
				_player.CurrentState = new DriveCartState(cart);
				RemoveOutline();
			} 
			else if (currentState.GetType() == typeof(DriveCartState)) {
				_player.CurrentState = new WalkRunState();
				AddOutline();
			}
		}

		private void AddOutline() {
			foreach (MeshRenderer r in _model.GetComponentsInChildren<MeshRenderer>()) {
				r.gameObject.layer = LayerMaskManager.Get(Layer.Outline);
			}
			
		}

		private void RemoveOutline() {
			foreach (MeshRenderer r in _model.GetComponentsInChildren<MeshRenderer>()) {
				r.gameObject.layer = LayerMaskManager.Get(Layer.Default);
			}
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return; 

			Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			use.StartBehaviour = () => {
				ToggleWalkDrive(transform.parent.gameObject); 
			};

			AddOutline();
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			RemoveOutline();
		}
	}
}
