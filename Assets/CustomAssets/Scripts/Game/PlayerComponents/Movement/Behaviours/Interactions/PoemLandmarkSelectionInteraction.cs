using Game.Poems;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {

	public class VersesDisplayer {
		public float VerticalSeparation = 1.0f;
		public Color BaseColor = Color.white;
		public Color ShadowColor = Color.black;
		public float Scale = 0.25f;
		public int FontSize = 60;
		public float ShadowOffset = 0.05f;

		private GameObject _container;

		private TextMesh[] _displayMeshText = new TextMesh[6];
		private TextMesh[] _shadowMeshText = new TextMesh[6];

		public VersesDisplayer() {
			_container = new GameObject("_textMeshContainer");
			Font font = Resources.Load<Font>("Fonts/HammerheadRegular");
			Material fontMat = Resources.Load<Material>("Fonts/HammerheadRegularMat");

			for (int i = 0; i < 6; ++i) {
				GameObject child = new GameObject("_child"+i);
				child.transform.parent = _container.transform;
				child.transform.LocalReset();
				_displayMeshText[i] = child.AddComponent<TextMesh>();
				_displayMeshText[i].fontSize = FontSize;
				_displayMeshText[i].transform.localScale = Vector3.one * Scale;
				_displayMeshText[i].alignment = TextAlignment.Center;
				_displayMeshText[i].anchor = TextAnchor.MiddleCenter;
				_displayMeshText[i].font = font;
				_displayMeshText[i].GetComponent<Renderer>().material = fontMat;
				_displayMeshText[i].GetComponent<Renderer>().material.color = BaseColor;

				GameObject subchild = new GameObject("_shadow"+i);
				subchild.transform.parent = child.transform;
				subchild.transform.LocalReset();
				_shadowMeshText[i] = subchild.AddComponent<TextMesh>();
				_shadowMeshText[i].fontSize = FontSize;
				_shadowMeshText[i].transform.localScale = Vector3.one;
				_shadowMeshText[i].offsetZ = ShadowOffset;
				_shadowMeshText[i].alignment = TextAlignment.Center;
				_shadowMeshText[i].anchor = TextAnchor.MiddleCenter;
				_shadowMeshText[i].font = font;
				_shadowMeshText[i].GetComponent<Renderer>().material = fontMat;
				_shadowMeshText[i].GetComponent<Renderer>().material.color = ShadowColor;
			}
		}

		public void Display(Vector3 position, Quaternion rotation, string[] verses) {
			_container.transform.position = position;
			_container.transform.rotation = rotation;
			Vector3 upperPosition =  Vector3.up * 2.5f * VerticalSeparation;
			position = upperPosition;
			for (int i = 0; i < 6; ++i) {
				_displayMeshText[i].gameObject.transform.localPosition = position;
				_displayMeshText[i].text = verses[i];
				_shadowMeshText[i].text = verses[i];
				position -= Vector3.up * VerticalSeparation;
			}
		}
	}

	public class PoemLandmarkSelectionInteraction : Interaction {

		private Transform _landmarkVisuals = null;
		private static readonly int _outlineLayer = LayerMaskManager.Get(Layer.Outline);
		private static readonly int _defaultLayer = LayerMaskManager.Get(Layer.Default);
		private LandmarkVersesComponent _verses;
		private string[] _versesText = new string[6];
		private static VersesDisplayer _displayMeshText = new VersesDisplayer();
		private enum PoemState {
			Init, AlreadyInit
		}
		private PoemState _state = PoemState.Init;

		private void DisplayVerses() {
			_versesText[0] = _verses.LandmarkVerses.Masculine[0];
			_versesText[1] = _verses.LandmarkVerses.Masculine[1];
			_versesText[2] = _verses.LandmarkVerses.Feminine[2];
			_versesText[3] = _verses.LandmarkVerses.Feminine[3];
			_versesText[4] = _verses.LandmarkVerses.Plural[4];
			_versesText[5] = _verses.LandmarkVerses.Plural[5];

			Player player = Player.GetInstance();
			_displayMeshText.Display(player.transform.position + player.transform.forward * 6.0f, player.transform.rotation, _versesText); // TODO parametrice distance
		}

		public override void DoInteraction() {

			if (_state == PoemState.Init) {
				Debug.Log(_verses.LandmarkVerses.Masculine[0]);
				_state = PoemState.AlreadyInit;
				DisplayVerses();
			}
			else {
				
			}

		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			if (Player.GetInstance().GetEyeSight(out hit)) {
				Debug.DrawLine(Player.GetInstance().transform.position, hit.point, Color.magenta);
				if (hit.collider.gameObject.tag == TagManager.Get(Tag.Landmark)) {
					_landmarkVisuals = hit.collider.gameObject.transform.parent;
					Transform landmarkParent = _landmarkVisuals.transform.parent;
					_verses = landmarkParent.GetComponent<LandmarkVersesComponent>();
					ShowFeedback();
				}
				else {
					HideFeedback();
					_landmarkVisuals = null;
					_verses = null;
				}
				
			}
			else {
				Debug.DrawRay(Player.GetInstance().transform.position, Player.GetInstance().Camera.transform.forward * 1000.0f, Color.cyan);
				HideFeedback();
				_landmarkVisuals = null;
				_verses = null;
			}
			return this;
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