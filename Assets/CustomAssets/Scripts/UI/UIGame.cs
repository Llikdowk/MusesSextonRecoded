
using UnityEngine;
using UnityEngine.UI;

namespace UI {

	public class UIGame : CoroutineBase {
		private static UIGame _instance = null;

		private Transform _canvas;
		private GameObject _crosshair;
		private Image _blackImage;

		public static UIGame GetInstance() {
			return _instance;
		}

		public void Awake() {
			if (_instance == null) {
				_instance = this;
				_canvas = transform.FindChild("Canvas");
				if (!_canvas) {
					DebugMsg.GameObjectNotFound(Debug.LogError, "Canvas");
				}

				foreach (Transform child in _canvas.transform.GetComponentsInChildren<Transform>()) {
					if (child.name.Equals("Crosshair")) {
						_crosshair = child.gameObject;
					}
				}
				GameObject black = new GameObject("_blackImage");
				black.transform.parent = _canvas.transform;
				black.transform.LocalReset();
				_blackImage = black.AddComponent<Image>();
				_blackImage.color = new Color(0,0,0,0);
				DontDestroyOnLoad(this);
			} else {
				Destroy(gameObject);
			}
		}

		public void ShowCrosshair(bool enabled) {
			_crosshair.SetActive(enabled);
		}

		public void FadeOut(Image image, float duration_s, CoroutineCallback callback = null) {
			Color start = image.color;
			Color end = new Color(image.color.r, image.color.g, image.color.b, 0);
			StartCoroutineNoTimescaled(duration_s,
				(t) => {
					image.color = Color.Lerp(start, end, t);
				}, 
				callback
			);
		}

		public void FadeIn(Image image, float duration_s, CoroutineCallback callback = null) {
			Color start = image.color;
			Color end = new Color(image.color.r, image.color.g, image.color.b, 1);
			StartCoroutineNoTimescaled(duration_s,
				(t) => {
					image.color = Color.Lerp(start, end, t);
				}, 
				callback
			);
		}

		public void FadeToBlack(float duration_s, CoroutineCallback callback = null) {
			_blackImage.color = new Color(0,0,0,0);
			FadeIn(_blackImage, duration_s, callback);
		}
		public void FadeFromBlack(float duration_s, CoroutineCallback callback = null) {
			_blackImage.color = new Color(0,0,0,1);
			FadeOut(_blackImage, duration_s, callback);
		}

	}
}
