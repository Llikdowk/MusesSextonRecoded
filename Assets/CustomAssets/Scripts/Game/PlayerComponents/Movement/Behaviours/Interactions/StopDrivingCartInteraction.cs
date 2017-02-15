using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class StopDrivingCartInteraction : Interaction {
		private DriveCartInteraction _driveInteraction;

		public StopDrivingCartInteraction(DriveCartInteraction driveInteraction) {
			_driveInteraction = driveInteraction;
		}

		public override void DoInteraction() {
			Debug.Log("stop driving!");
			Player.GetInstance().CurrentState = new WalkRunStateFromCart(_driveInteraction);
			//Player.GetInstance().Movement.MovementBehaviour.AvailableInteractions.Insert(0, new DriveCartInteraction());
		}

		public override Interaction CheckForPromotion() {
			Debug.Log("check for promotion");
			return this;
		}
	}
}