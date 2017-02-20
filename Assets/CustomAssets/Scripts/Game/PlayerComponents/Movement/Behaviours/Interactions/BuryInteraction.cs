using Game.Entities;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class BuryInteraction : Interaction {
		private const float _upGroundStep = 0.60f;
		private const float _upGravestoneStep = 1.5f;
		private readonly GameObject _tomb;
		private readonly GameObject _gravestone;
		private readonly GameObject _ground;
		private int _counter = 0;
		private const int MaxCount = 3;

		public BuryInteraction(GameObject tomb, GameObject ground) {
			_ground = ground;
			_tomb = tomb;
			_gravestone = tomb.GetComponent<TombComponent>().Gravestone;
		}

		public override void DoInteraction() {
			if (Player.GetInstance().PlayDigAnimation()) {
				_ground.transform.position += _ground.transform.up * _upGroundStep;
				_gravestone.transform.position += _gravestone.transform.up * _upGravestoneStep;
				++_counter;
				if (_counter >= MaxCount) {
					Player.GetInstance().AnimationEnding = () => {
						++GameState.CoffinsBuried;
						Player.GetInstance().CurrentState = new PoemState();
						_ground.transform.parent = null;
						_gravestone.transform.parent = null;
						_tomb.SetActive(false);
					};
				}
			}
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}