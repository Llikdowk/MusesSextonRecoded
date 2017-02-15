using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class SendCoffinToTombInteraction : Interaction {

		private GameObject _tomb;
		private GameObject _ground;
		private GameObject _coffin;

		public SendCoffinToTombInteraction(GameObject tomb, GameObject ground, GameObject coffin) {
			_tomb = tomb;
			_coffin = coffin;
			_ground = ground;
		}

		public override void DoInteraction() {
			_coffin.transform.parent = _tomb.transform;
			_coffin.transform.LocalReset();
			Player.GetInstance().CurrentState = new BuryState(_tomb, _ground);
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}