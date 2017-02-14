using Game;
using Game.PlayerComponents;
using MiscComponents;
using UnityEngine;

namespace Triggers {

	using Action = Action<PlayerAction>;

	public class TFrontCart : MonoBehaviour {

		private Player _player;
		private GameObject _model;
		private MarkableComponent mark;


		public void Start() {
			_player = Player.GetInstance();
			foreach (Transform t in transform.parent.GetComponentInChildren<Transform>()) {
				if (t.gameObject.name == "Model") {
					_model = t.gameObject;
					break;
				}
			}
			mark = transform.parent.gameObject.GetComponent<MarkableComponent>();
			if (mark == null) DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));


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

			if (mark) mark.DisableMark();
			if (_player.CurrentState.GetType() == typeof(WalkRunState)) {
				_player.CurrentState.CheckInternalInteraction(false);

				if (_player.CurrentState.GetType() == typeof(WalkRunState)) {
					AddOutline();
					Action use = _player.Actions.GetAction(PlayerAction.Use);
					use.StartBehaviour = () => {
						RemoveOutline();
						_player.CurrentState = new DriveCartState(transform.parent.gameObject);
					};
				}
			}
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			_player.CurrentState.CheckInternalInteraction(true);
			if (mark) mark.EnableMark();
			RemoveOutline();
		}
	}
}
