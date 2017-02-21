
using Game.Entities;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class DigInteraction : Interaction {
		private readonly TombComponent _tombComponent;
		private int _counter = 0;
		private const int MaxCount = 2;

		public DigInteraction(TombComponent tombComponent) {
			_tombComponent = tombComponent;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				++_counter;
				_tombComponent.Dig();
				if (_counter >= MaxCount) {
					Player.GetInstance().AnimationEnding = 
						() => Player.GetInstance().CurrentState = new WalkRunState();
				}
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}
