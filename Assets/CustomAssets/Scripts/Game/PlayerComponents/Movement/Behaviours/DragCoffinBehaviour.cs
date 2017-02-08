using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class DragCoffinBehaviour : WalkRunMovementBehaviour {
		public DragCoffinBehaviour(Transform transform, SuperConfig config) : base(transform, config) {
			Action<PlayerAction> _useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);

			_useAction.StartBehaviour = () => {
					Player.GetInstance().CurrentState = new WalkRunState();
			};
		}

		protected override void Calcs() {
			
		}
	}

}
