using Game.Entities;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class BuryInteraction : Interaction {
		private readonly TombComponent _tombComponent;
		private int _counter = 0;
		private const int MaxCount = 3;

		public BuryInteraction(TombComponent tombComponent) {
			_tombComponent = tombComponent;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				_tombComponent.Bury();
				++_counter;
				if (_counter >= MaxCount) {
					Player.GetInstance().AnimationEnding = () => {
						++GameState.CoffinsBuried;
						Player.GetInstance().CurrentState = new PoemState();
						_tombComponent.Hide();
					};
				}
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}