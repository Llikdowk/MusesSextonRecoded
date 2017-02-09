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

		private ActionDelegate _useStartBehaviourBackup;


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

		private void ToggleWalkDrive(GameObject cart) {
			PlayerState currentState = _player.CurrentState;

			if (currentState.GetType() == typeof(WalkRunState)) {
				_player.CurrentState = new DriveCartState(cart);
				RemoveOutline();
			} 
			else if (currentState.GetType() == typeof(DriveCartState)) {
				_player.CurrentState = new WalkRunState();
				OnTriggerEnter(Player.GetInstance().GetComponent<Collider>());
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
			if (Player.GetInstance().CurrentState.GetType() != typeof(WalkRunState)) return;

			if (mark) mark.DisableMark();
			Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			_useStartBehaviourBackup = use.StartBehaviour.Clone() as ActionDelegate;
			if (_useStartBehaviourBackup == null) _useStartBehaviourBackup = Action.nop;
			use.StartBehaviour = () => {
				ToggleWalkDrive(transform.parent.gameObject);
			};

			AddOutline();
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			if (Player.GetInstance().CurrentState.GetType() != typeof(WalkRunState)) return;

			if (mark) mark.EnableMark();
			Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			use.StartBehaviour = _useStartBehaviourBackup; // BUG fixme! disables all use actions in the future
			RemoveOutline();
		}
	}
}
