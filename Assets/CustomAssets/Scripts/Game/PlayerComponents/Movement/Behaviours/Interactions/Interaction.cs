
namespace Game.PlayerComponents.Movement.Behaviours.Interactions {

	public abstract class Interaction {

		public abstract void DoInteraction();
		protected abstract void ShowFeedback();
		public abstract void HideFeedback();

		public abstract Interaction Check();
	}
}
