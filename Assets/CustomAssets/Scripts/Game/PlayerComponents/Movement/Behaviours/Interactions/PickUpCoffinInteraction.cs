using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PickUpCoffinInteraction : Interaction {
		public GameObject Coffin;
		private int _backupLayer = LayerMaskManager.Get(Layer.Default);
		
		protected override void ShowFeedback() {
			Coffin.layer = LayerMaskManager.Get(Layer.Outline);
		}

		public override void HideFeedback() {
			Coffin.layer = _backupLayer;
		}

		public override Interaction CheckForPromotion() {
			Player player = Player.GetInstance();
			RaycastHit hit;
			bool hasHit = player.GetEyeSight(out hit);
			if (!hasHit) return null;
			GameObject g = hit.collider.gameObject;
			if (g.tag == TagManager.Get(Tag.Coffin) && hit.distance < 2.5f) {
				if (Coffin != null) { HideFeedback(); }
				Coffin = g;
				_backupLayer = g.layer;
				ShowFeedback();
				return this;
			}
			return null;
		}

		public override void DoInteraction() {
			Debug.Log("DO PICK UP INTERACTION");
			Coffin.layer = LayerMaskManager.Get(Layer.DrawFront);
			MarkableComponent m = Coffin.GetComponent<MarkableComponent>();
			if (m != null) {
				m.DisableMark();
			} else {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));
			}
			Player.GetInstance().CurrentState = new DragCoffinState(Coffin);
		}
	}
}