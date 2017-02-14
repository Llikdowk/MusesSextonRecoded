using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class DigBehaviour : MovementBehaviour {
		public DigBehaviour(Transform transform, GameObject ground) : base(transform) {
			var groundTransform = ground.transform;
			Action<PlayerAction> useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			const int maxCount = 3;
			int count = 0;
			useAction.StartBehaviour = () => {
				groundTransform.position -= groundTransform.up;
				++count;
				if (count >= maxCount) {
					Player.GetInstance().CurrentState = new WalkRunState();
				}
			};
		}

		public override void Step() {
		}

		public override void OnDestroy() {
		}
	}

}
