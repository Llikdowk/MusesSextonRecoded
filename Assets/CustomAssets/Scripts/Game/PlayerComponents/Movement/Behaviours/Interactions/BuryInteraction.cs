using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class BuryInteraction : Interaction {
		private const float _upStep = 0.60f;
		private readonly GameObject _tomb;
		private readonly GameObject _ground;
		private int _counter = 0;
		private const int MaxCount = 3;

		public BuryInteraction(GameObject tomb, GameObject ground) {
			_ground = ground;
			_tomb = tomb;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				_ground.transform.position += _ground.transform.up * _upStep;
				++_counter;
				if (_counter >= MaxCount) {
					Player.GetInstance().CurrentState = new PoemState();
					_ground.transform.parent = null;
					_tomb.SetActive(false);
				}
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}