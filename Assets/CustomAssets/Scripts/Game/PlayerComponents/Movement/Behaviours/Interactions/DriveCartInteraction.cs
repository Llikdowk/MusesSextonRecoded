
using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class DriveCartInteraction : Interaction {
		private readonly GameObject _cart;
		private readonly GameObject _model;
		private readonly MarkableComponent _mark;

		public DriveCartInteraction(GameObject cart) {
			_cart = cart;
			foreach (Transform t in cart.GetComponentInChildren<Transform>()) {
				if (t.gameObject.name == "Model") {
					_model = t.gameObject;
					break;
				}
			}
			_mark = cart.GetComponent<MarkableComponent>();
			if (_mark == null) DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));
		}

		public override void DoInteraction() {
			Player.GetInstance().CurrentState = new DriveCartState(_cart);
		}

		protected override void ShowFeedback() {
			_mark.DisableMark();
			foreach (MeshRenderer r in _model.GetComponentsInChildren<MeshRenderer>()) {
				r.gameObject.layer = LayerMaskManager.Get(Layer.Outline);
			}
		}

		public override void HideFeedback() {
			_mark.EnableMark();
			foreach (MeshRenderer r in _model.GetComponentsInChildren<MeshRenderer>()) {
				r.gameObject.layer = LayerMaskManager.Get(Layer.Default);
			}
		}

		public override Interaction Check() {
			ShowFeedback();
			return this;
		}
	}
}