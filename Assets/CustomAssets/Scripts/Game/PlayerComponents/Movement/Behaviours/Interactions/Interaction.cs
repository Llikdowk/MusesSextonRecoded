namespace Game.PlayerComponents.Movement.Behaviours.Interactions {

	public abstract class Interaction {
		public abstract void DoInteraction();
		public abstract Interaction CheckForPromotion();

		public virtual void ShowFeedback() {}
		public virtual void HideFeedback() {}
	}
}
