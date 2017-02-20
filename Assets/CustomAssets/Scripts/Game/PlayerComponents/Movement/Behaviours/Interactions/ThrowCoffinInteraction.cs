using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class ThrowCoffinInteraction : Interaction {
		private readonly GameObject _coffin;

		public ThrowCoffinInteraction(GameObject coffin) {
			_coffin = coffin;
		}

		public override void DoInteraction() {
			_coffin.transform.parent = null;
			_coffin.GetComponent<Collider>().enabled = true;
			Rigidbody rb = _coffin.GetComponent<Rigidbody>();
			rb.isKinematic = false;
			_coffin.layer = LayerMaskManager.Get(Layer.Default);

			MarkableComponent m = _coffin.GetComponent<MarkableComponent>();
			if (m == null) {
				Debug.LogWarning("Object <b>" + _coffin.gameObject.name + "</b> could not be marked. (Missing MarkableComponent!)"); // TODO extract to Error class
			}
			else {
				//TODO: ADD GUARD -> enable only if not throwed into a hole
				m.enabled = true;
			}

			Player.GetInstance().CurrentState = new WalkRunState();
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}