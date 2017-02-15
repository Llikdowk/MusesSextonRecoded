
namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class EmptyInteraction : Interaction {
		public override void DoInteraction() {
		}

		protected override void ShowFeedback() {
		}

		public override void HideFeedback() {
		}

		public override Interaction CheckForPromotion() {
			return null;
		}
	}
}
