using Game.Poems;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PoemLandmarkSelectionInteraction : Interaction {

		private static readonly int _outlineLayer = LayerMaskManager.Get(Layer.Outline);
		private static readonly int _defaultLayer = LayerMaskManager.Get(Layer.Default);
		private Transform _landmarkVisuals = null;
		private LandmarkVersesComponent _verses;

		public override void DoInteraction() {
			((PoemState)Player.GetInstance().CurrentState).SetVerseInteraction(_verses.LandmarkVerses);

		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			if (Player.GetInstance().GetEyeSight(out hit)) {
				Debug.DrawLine(Player.GetInstance().transform.position, hit.point, Color.magenta);
				if (hit.collider.gameObject.tag == TagManager.Get(Tag.Landmark)) {
					GameObject LandmarkParent = null;
					Transform current = hit.collider.transform;
					while (current.parent != null && !current.parent.name.Contains("Landmark")) {
						current = current.parent;
					}
					if (current != null) {
						_landmarkVisuals = current;
						Transform landmarkParent = _landmarkVisuals.transform.parent;
						_verses = landmarkParent.GetComponent<LandmarkVersesComponent>();
						ShowFeedback();
						return this;
					}
				}
			}

			Debug.DrawRay(Player.GetInstance().transform.position, Player.GetInstance().Camera.transform.forward * 1000.0f, Color.cyan);
			HideFeedback();
			return null;
		}

		public override void ShowFeedback() {
			if (!_landmarkVisuals) return;

			foreach (Transform child in _landmarkVisuals.GetComponentsInChildren<Transform>()) {
				child.gameObject.layer = _outlineLayer;
			}
		}

		public override void HideFeedback() {
			if (!_landmarkVisuals) return;
			foreach (Transform child in _landmarkVisuals.GetComponentsInChildren<Transform>()) {
				child.gameObject.layer = _defaultLayer;
			}
		}
	}
}