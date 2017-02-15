using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PoemInteraction : Interaction {
		public override void DoInteraction() {
			Debug.Log("POEM MODE");
		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			if (Player.GetInstance().GetEyeSight(out hit)) {
				Debug.DrawLine(Player.GetInstance().transform.position, hit.point, Color.magenta);
			}
			else {
				Debug.DrawRay(Player.GetInstance().transform.position, Player.GetInstance().Camera.transform.forward * 1000.0f, Color.cyan);
			}
			return this;
		}

		public override void ShowFeedback() { }
		public override void HideFeedback() { }
	}
}