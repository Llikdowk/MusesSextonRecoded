using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PickUpCoffinInteraction : Interaction {
		public GameObject Coffin;
		private readonly int _backupLayer;

		public PickUpCoffinInteraction(GameObject coffin) {
			Debug.Log("CONSTRUCT COFFIN INTERACTION");
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

		public override void ShowFeedback() {
			Coffin.layer = LayerMaskManager.Get(Layer.Outline);
		}

		public override void HideFeedback() {
			Coffin.layer = _backupLayer;
		}
	}
}