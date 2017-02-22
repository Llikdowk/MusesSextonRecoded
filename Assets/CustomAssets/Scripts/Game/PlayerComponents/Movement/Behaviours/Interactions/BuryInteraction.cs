using Game.Entities;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class BuryInteraction : Interaction {
		private readonly TombComponent _tombComponent;
		private static int _counter = 0;
		private const int MaxCount = 3;

		public BuryInteraction(TombComponent tombComponent) {
			_tombComponent = tombComponent;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				_tombComponent.Bury();
				++_counter;
				Player.GetInstance().AnimationEnding = () => {
					Player.GetInstance().CurrentState = new PoemState(_tombComponent);
				};
				if (_counter >= MaxCount) {
					Player.GetInstance().AnimationEnding = () => {
						++GameState.CoffinsBuried;
						Player.GetInstance().CurrentState = new WalkRunState();
						_tombComponent.Hide();
					};
					_counter = 0;
				}
			}
		}

		public override Interaction CheckForPromotion() {
			DoInteraction();
			return null;
		}
	}
}