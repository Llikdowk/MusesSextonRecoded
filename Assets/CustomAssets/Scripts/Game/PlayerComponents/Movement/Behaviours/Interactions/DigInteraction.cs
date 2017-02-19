
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class DigInteraction : Interaction {
		private readonly GameObject _ground;
		private int _counter = 0;
		private const int MaxCount = 2;

		public DigInteraction(GameObject ground) {
			_ground = ground;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				_ground.transform.position -= _ground.transform.up;
				++_counter;
				if (_counter >= MaxCount) {
					Player.GetInstance().AnimationEnding = 
						() => Player.GetInstance().CurrentState = new WalkRunState();
				}
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}
