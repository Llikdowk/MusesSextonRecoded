using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class BuryInteraction : Interaction {
		private readonly GameObject _tomb;
		private readonly GameObject _ground;
		private int _counter = 0;
		private const int MaxCount = 3;

		public BuryInteraction(GameObject tomb, GameObject ground) {
			_ground = ground;
			_tomb = tomb;
		}

		public override void DoInteraction() {
			_ground.transform.position += _ground.transform.up;
			++_counter;
			if (_counter >= MaxCount) {
				Player.GetInstance().CurrentState = new WalkRunState();
				_ground.transform.parent = null;
				_tomb.SetActive(false);
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}