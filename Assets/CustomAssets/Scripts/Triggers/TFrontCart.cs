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

		//private ActionDelegate _useStartBehaviourBackup;


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
			if (Player.GetInstance().CurrentState.GetType() == typeof(WalkRunState)) {
				Player.GetInstance().CurrentState.CheckInternalInteraction(false);

				if (Player.GetInstance().CurrentState.GetType() == typeof(WalkRunState)) {
					AddOutline();
					Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
					use.StartBehaviour = () => {
						RemoveOutline();
						Player.GetInstance().CurrentState = new DriveCartState(transform.parent.gameObject);
					};
				}
			}
			Debug.Log("ontriggerenter");
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			Player.GetInstance().CurrentState.CheckInternalInteraction(true);
			if (mark) mark.EnableMark();
			RemoveOutline();
			Debug.Log("ontriggerexit");
		}

		public void OnTriggerStay(Collider other) {
			Debug.Log("ontriggerstay " + other.gameObject.name);
		}
	}
}
