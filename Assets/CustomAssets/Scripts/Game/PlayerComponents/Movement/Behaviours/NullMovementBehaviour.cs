using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class NullMovementBehaviour : MovementBehaviour {
		public override Vector3 SelfMovement { get { return Vector3.zero; } }
		public override Vector3 WorldMovement { get { return Vector3.zero; } }
		public override Vector3 SelfDir { get { return Vector3.zero; } }
		public override Vector3 WorldDir { get { return Vector3.zero; } }

		public NullMovementBehaviour() : base(null) {
		}

		public override void OnDestroy() {
		}
	}

}
