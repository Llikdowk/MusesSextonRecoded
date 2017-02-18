using Game.Entities;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class SendCoffinToTombInteraction : Interaction {

		private readonly GameObject _tomb;
		private readonly TombComponent _tombComponent;
		private readonly GameObject _ground;
		private readonly GameObject _coffin;

		public SendCoffinToTombInteraction(GameObject tomb, GameObject ground, GameObject coffin) {
			_tomb = tomb;
			_coffin = coffin;
			_ground = ground;
			_tombComponent = _tomb.GetComponent<TombComponent>();
		}

		public override void DoInteraction() {
			_coffin.transform.parent = _tomb.transform;
			_coffin.transform.LocalReset();
			Player.GetInstance().CurrentState = new BuryState(_tomb, _ground);
		}

		public override void ShowFeedback() {
			_tombComponent.ShowActionIcon(true);
		}

		public override void HideFeedback() {
			_tombComponent.ShowActionIcon(false);
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}