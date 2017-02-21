using Audio;
using Game.Entities;
using UnityEngine;
using Utils;
using Debug = System.Diagnostics.Debug;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class SendCoffinToTombInteraction : Interaction {

		private readonly GameObject _tomb;
		private readonly TombComponent _tombComponent;
		private readonly GameObject _ground;
		private readonly GameObject _coffin;

		public SendCoffinToTombInteraction(GameObject tomb, GameObject ground, GameObject coffin) {
			_tomb = tomb;
			_coffin = coffin;
			_ground = ground;
			_tombComponent = _tomb.GetComponent<TombComponent>();
		}

		public override void DoInteraction() {
			_coffin.layer = LayerMaskManager.Get(Layer.Default);
			AnimationUtils.MoveSmoothlyTo(_coffin.transform, _coffin.transform.position, _tomb.transform.position - Vector3.up*1.5f, 0.5f);
			AnimationUtils.SlerpTowards(_coffin.transform, _coffin.transform.forward, _tomb.transform.right, 0.5f, () => {
				_coffin.transform.parent = _tomb.transform;
			});
			_tombComponent.PlayerTombTransition(new BuryState(_tomb, _ground));
			AudioController.GetInstance().AddMusicChannel();
		}

		public override void ShowFeedback() {
			_tombComponent.ShowActionIcon(true);
		}

		public override void HideFeedback() {
			_tombComponent.ShowActionIcon(false);
		}

		public override Interaction CheckForPromotion() {
			return this;
		}
	}
}