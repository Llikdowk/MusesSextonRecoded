
namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class EmptyInteraction : Interaction {
		public override void DoInteraction() {
		}

		public override Interaction CheckForPromotion() {
			return null;
		}
	}
}
