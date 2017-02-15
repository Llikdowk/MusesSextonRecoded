using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PickUpCoffinInteraction : Interaction {
		public GameObject Coffin;
		private int _backupLayer;
		private EyeSight _eyeSight;


		public PickUpCoffinInteraction(EyeSight eyeSight) {
			_eyeSight = eyeSight;
		}

		private void Init(GameObject coffin) {
			Debug.Log("INIT COFFIN INTERACTION");
			Coffin = coffin;
			_backupLayer = coffin.layer;
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

		protected override void ShowFeedback() {
			Coffin.layer = LayerMaskManager.Get(Layer.Outline);
		}

		public override void HideFeedback() {
			Coffin.layer = _backupLayer;
		}

		public override Interaction Check() {
			if (!_eyeSight.HasHit) return null;
			GameObject g = _eyeSight.Hit.collider.gameObject;
			if (g.tag == TagManager.Get(Tag.Coffin) && _eyeSight.Hit.distance < 2.5f) {
				if (Coffin != null) { HideFeedback(); }
				Init(g);
				ShowFeedback();
				return this;
			}
			return null;
		}
	}
}