
namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class StopDrivingCartInteraction : Interaction {

		public override void DoInteraction() {
			Player.GetInstance().CurrentState = new WalkRunState();
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}