using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class NullMovementBehaviour : MovementBehaviour {
		public NullMovementBehaviour(Transform transform) : base(transform) {
		}

		public override void Step() {
		}

		public override void ResetModifiedState() {
		}
	}

}
