using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class VersesDisplayer {
		public static VersesDisplayer NormalPoem = new VersesDisplayer(6);
		public static VersesDisplayer PlayerPoem = new VersesDisplayer(3);

		public float VerticalSeparation = 1.65f;
		public Color BaseColor = Color.white;
		public Color BaseHighlightColor = Color.red;
		public Color ShadowColor = Color.black;
		public float Scale = 0.25f;
		public int FontSize = 60;
		public float ShadowOffset = 0.4f;

		private GameObject _container;

		private readonly TextMesh[] _displayMeshText;
		private readonly TextMesh[] _shadowMeshText;

		public VersesDisplayer(int length) {
			_displayMeshText = new TextMesh[length];
			_shadowMeshText = new TextMesh[length];

			_container = new GameObject("_textMeshContainer");
			Font font = Resources.Load<Font>("Fonts/HammerheadRegular");
			Material fontMat = Resources.Load<Material>("Fonts/HammerheadRegularAlwaysFrontMat");

			for (int i = 0; i < _displayMeshText.Length; ++i) {
				GameObject child = new GameObject("_child"+i);
				child.layer = LayerMaskManager.Get(Layer.Verse);
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
				subchild.layer = LayerMaskManager.Get(Layer.Verse);
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

				_displayMeshText[i].gameObject.SetActive(false);
			}
		}

		public void Display(Vector3 position, Quaternion rotation, VerseInfo[] verses) {
			_container.transform.position = position;
			_container.transform.rotation = rotation;
			Vector3 upperPosition =  Vector3.up * 2.5f * VerticalSeparation;
			position = upperPosition;
			for (int i = 0; i < _displayMeshText.Length; ++i) {
				_displayMeshText[i].gameObject.SetActive(true);
				_displayMeshText[i].gameObject.transform.localPosition = position;
				_displayMeshText[i].text = verses[i].Verse;
				_displayMeshText[i].gameObject.AddComponent<BoxCollider>().isTrigger = true;
				_displayMeshText[i].gameObject.GetOrAddComponent<VerseInfoComponent>().Info = verses[i];
				_shadowMeshText[i].text = verses[i].Verse;
				position -= Vector3.up * VerticalSeparation;
			}
		}

		public void Hide() {
			for (int i = 0; i < _displayMeshText.Length; ++i) {
				Object.Destroy(_displayMeshText[i].gameObject.GetComponent<BoxCollider>());
				_displayMeshText[i].gameObject.SetActive(false);
			}
			
		}
	}
}