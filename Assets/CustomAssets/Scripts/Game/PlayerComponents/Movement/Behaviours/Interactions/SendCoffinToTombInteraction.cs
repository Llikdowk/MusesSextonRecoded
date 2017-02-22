using Audio;
using Game.Entities;
using UnityEngine;
using Utils;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class SendCoffinToTombInteraction : Interaction {

		private readonly GameObject _tomb;
		private readonly TombComponent _tombComponent;
		private readonly GameObject _coffin;

		public SendCoffinToTombInteraction(TombComponent tombComponent, GameObject coffin) {
			_coffin = coffin;
			_tombComponent = tombComponent;
			_tomb = _tombComponent.gameObject;
		}

		public override void DoInteraction() {
			_coffin.layer = LayerMaskManager.Get(Layer.Landmark);
			AnimationUtils.MoveSmoothlyTo(_coffin.transform, _coffin.transform.position, _tomb.transform.position - Vector3.up*1.5f, 0.5f);
			AnimationUtils.LookTowardsHorizontal(_coffin.transform, _tomb.transform.right, 0.5f, () => {
				_coffin.transform.parent = _tomb.transform;
			});
			_tombComponent.PlayerTombTransition(new PoemState(_tombComponent), false);
			_tombComponent.HideMarker();
			_tombComponent.HideColliders();
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