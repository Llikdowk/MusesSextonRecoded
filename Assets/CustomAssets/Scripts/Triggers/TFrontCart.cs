using System.Linq;
using Game;
using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {

	using Action = Action<PlayerAction>;

	public class TFrontCart : MonoBehaviour {

		private ActionDelegate[] _backupUseBehaviour;
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

			if (currentState.GetType() == typeof(WalkState)) {
				_player.CurrentState = new DriveCartState(cart);
				RemoveOutline();
			} 
			else if (currentState.GetType() == typeof(DriveCartState)) {
				_player.CurrentState = new WalkState();
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
			Debug.Log("ontriggerenter");

			Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			_backupUseBehaviour = use.GetAllFunctions();
			use.StartBehaviour = () => {
				ToggleWalkDrive(transform.parent.gameObject); 
			};

			AddOutline();
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;
			Debug.Log("ontriggerexit");

			Player.GetInstance().Actions.GetAction(PlayerAction.Use).SetAllFunctions(_backupUseBehaviour);
			RemoveOutline();

		}
	}
}
