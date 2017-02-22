using Audio;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class SaveCoffinInCartInteraction : Interaction {
		private readonly GameObject _coffin;
		private readonly Transform _slot;

		public SaveCoffinInCartInteraction(GameObject coffin, Transform slot) {
			_coffin = coffin;
			_slot = slot;
		}

		public override void DoInteraction() {
			AudioController.GetInstance().PlayPickupCoffin();
			_coffin.transform.parent = _slot;
			_coffin.transform.LocalReset();
			_coffin.layer = LayerMaskManager.Get(Layer.Landmark);
			_coffin.GetComponent<Collider>().enabled = true;
			Player.GetInstance().CurrentState = new WalkRunState();
		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			if (Player.GetInstance().GetEyeSight(out hit)) {
				if (hit.collider.gameObject.tag == TagManager.Get(Tag.Terrain)) {
					return null;
				}
			}
			return this;
		}
	}
}