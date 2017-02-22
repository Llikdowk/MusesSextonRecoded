using Game.Poems;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class PoemLandmarkSelectionInteraction : Interaction {

		private static readonly int _outlineLayer = LayerMaskManager.Get(Layer.Outline);
		private static readonly int _defaultLayer = LayerMaskManager.Get(Layer.Landmark);
		private Transform _landmarkVisuals = null;
		private LandmarkVersesComponent _verses;

		public override void DoInteraction() {
			((PoemState)Player.GetInstance().CurrentState).SetVerseInteraction(_verses.LandmarkVerses);

		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			if (Player.GetInstance().GetEyeSight(out hit)) {
				Debug.DrawLine(Player.GetInstance().transform.position, hit.point, Color.magenta);
				if (hit.collider.gameObject.tag == TagManager.Get(Tag.Landmark)
				    || hit.collider.gameObject.tag == TagManager.Get(Tag.Coffin)) {
					Transform current = hit.collider.transform;
					if (!current.name.Contains("Landmark")) {
						while (current.parent != null && !current.parent.name.Contains("Landmark")) {
							current = current.parent;
						}
						if (current != null) {
							if (_landmarkVisuals != null && current.gameObject.GetInstanceID() != _landmarkVisuals.gameObject.GetInstanceID()) {
								HideFeedback();
							}
							_landmarkVisuals = current;
							Transform landmarkParent = _landmarkVisuals.transform.parent;
							_verses = landmarkParent.GetComponent<LandmarkVersesComponent>();
							ShowFeedback();
							return this;
						}
					}
					else {
						if (_landmarkVisuals != null && current.gameObject.GetInstanceID() != _landmarkVisuals.gameObject.GetInstanceID()) {
							HideFeedback();
						}
						_landmarkVisuals = current;
						_verses = current.GetComponent<LandmarkVersesComponent>();
						ShowFeedback();
						return this;
					}
				}
				else {
					HideFeedback();
				}
			}

			Debug.DrawRay(Player.GetInstance().transform.position, Player.GetInstance().MainCamera.transform.forward * 1000.0f, Color.cyan);
			HideFeedback();
			return null;
		}

		public override void ShowFeedback() {
			if (!_landmarkVisuals) return;

			_landmarkVisuals.gameObject.layer = _outlineLayer;
			foreach (Transform child in _landmarkVisuals.GetComponentsInChildren<Transform>()) {
				child.gameObject.layer = _outlineLayer;
			}
		}

		public override void HideFeedback() {
			if (!_landmarkVisuals) return;

			_landmarkVisuals.gameObject.layer = _defaultLayer;
			foreach (Transform child in _landmarkVisuals.GetComponentsInChildren<Transform>()) {
				child.gameObject.layer = _defaultLayer;
			}
		}
	}
}