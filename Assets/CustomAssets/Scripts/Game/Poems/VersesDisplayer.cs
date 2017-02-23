using System.Collections;
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
				_displayMeshText[i].gameObject.GetOrAddComponent<VerseInfoComponent>().Info = verses[i];
				_displayMeshText[i].GetComponent<Renderer>().material.color = BaseColor;

				_shadowMeshText[i].gameObject.transform.LocalReset();
				_shadowMeshText[i].text = verses[i].Verse;
				_shadowMeshText[i].GetComponent<Renderer>().material.color = ShadowColor;

				position -= Vector3.up * VerticalSeparation;

				SmoothTextTransitionComponent smooth = _displayMeshText[i].gameObject.GetOrAddComponent<SmoothTextTransitionComponent>();
				smooth.Init(_displayMeshText[i], _shadowMeshText[i], 0.5f, i*0.1f);
			}
		}

		public void Hide() {
			for (int i = 0; i < _displayMeshText.Length; ++i) {
				if (!_displayMeshText[i].gameObject.activeSelf) continue;
				Object.Destroy(_displayMeshText[i].gameObject.GetComponent<BoxCollider>());
				_displayMeshText[i].GetComponent<SmoothTextTransitionComponent>().RunHide(0.5f, i*0.1f);
			}
			
		}

		private class SmoothTextTransitionComponent : MonoBehaviour {
			private TextMesh _displayText;
			private TextMesh _shadowText;
			private const float yOffset = 0.5f;
			private Vector3 _displayStart;
			private Vector3 _shadowStart;
			private Vector3 _displayEnd;
			private Vector3 _shadowEnd;
			private Renderer _displayRenderer;
			private Renderer _shadowRenderer;
			private Color _startDisplayColor;
			private Color _startShadowColor;
			private Color _endDisplayColor;
			private Color _endShadowColor;

			public void Init(TextMesh displayText, TextMesh shadowText, float duration_s, float delay_s) {
				_displayText = displayText;
				_shadowText = shadowText;

				_displayEnd = _displayText.transform.position;
				_shadowEnd = _shadowText.transform.position;

				_displayText.transform.position += _displayText.transform.up * yOffset;
				_shadowText.transform.position += _shadowText.transform.up * yOffset;
				_displayStart = _displayText.transform.position;
				_shadowStart = _shadowText.transform.position;

				_displayRenderer = _displayText.GetComponent<Renderer>();
				_shadowRenderer = _shadowText.GetComponent<Renderer>();

				_endDisplayColor = _displayRenderer.material.color;
				_endShadowColor = _shadowRenderer.material.color;
				_startDisplayColor = new Color(_endDisplayColor.r, _endDisplayColor.g, _endDisplayColor.b, 0.0f);
				_displayRenderer.material.color = _startDisplayColor;
				_startShadowColor = new Color(_endShadowColor.r, _endShadowColor.g, _endShadowColor.b, 0.0f);
				_shadowRenderer.material.color = _startShadowColor;

				RunShow(duration_s, delay_s);
			}

			private void RunShow(float duration_s, float delay_s) {
				StartCoroutine(SmoothShow(duration_s, delay_s));
			}

			public void RunHide(float duration_s, float delay_s) {
				StartCoroutine(SmoothHide(duration_s, delay_s));
			}

			private IEnumerator SmoothShow(float duration_s, float delay_s) {
				yield return new WaitForSeconds(delay_s);
				float t = 0.0f;
				while (t < 1.0f) {
					_displayText.transform.position = Vector3.Lerp(_displayStart, _displayEnd, t);
					_shadowText.transform.position = Vector3.Lerp(_shadowStart, _shadowEnd, t);
					_displayRenderer.material.color = Color.Lerp(_startDisplayColor, _endDisplayColor, t);
					_shadowRenderer.material.color = Color.Lerp(_startShadowColor, _endShadowColor, t);

					t += Time.deltaTime / duration_s;
					yield return null;
				}
				_displayText.gameObject.AddComponent<BoxCollider>().isTrigger = true;
			}

			private IEnumerator SmoothHide(float duration_s, float delay_s) {
				yield return new WaitForSeconds(delay_s);
				float t = 0.0f;
				while (t < 1.0f) {
					_displayText.transform.position = Vector3.Lerp(_displayEnd, _displayStart, t);
					_shadowText.transform.position = Vector3.Lerp(_shadowEnd, _shadowStart, t);
					_displayRenderer.material.color = Color.Lerp(_endDisplayColor, _startDisplayColor, t);
					_shadowRenderer.material.color = Color.Lerp(_endShadowColor, _startShadowColor, t);

					t += Time.deltaTime / duration_s;
					yield return null;
				}
				gameObject.SetActive(false);
			}
			
		}
	}
}