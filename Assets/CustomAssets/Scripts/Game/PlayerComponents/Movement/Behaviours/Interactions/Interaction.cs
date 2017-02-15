using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {

	public abstract class Interaction {
		public abstract void DoInteraction();
		public abstract Interaction CheckForPromotion();

		public virtual void ShowFeedback() {}
		public virtual void HideFeedback() {}
	}

	public class SendCoffinToTombInteraction : Interaction {

		private GameObject _tomb;
		private GameObject _coffin;

		public SendCoffinToTombInteraction(GameObject tomb, GameObject coffin) {
			_tomb = tomb;
			_coffin = coffin;
		}

		public override void DoInteraction() {
			_coffin.transform.parent = _tomb.transform;
			_coffin.transform.LocalReset();
			Player.GetInstance().CurrentState = new WalkRunState();
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}
