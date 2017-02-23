
using UnityEngine;
using UnityEngine.UI;

namespace Utils {

	public class UIGame : MonoBehaviour {
		public delegate void UIAction();

		private static UIGame _instance = null;
		private GameObject Crosshair;
		private Image BlackImage;

		public static UIGame GetInstance() {
			return _instance;
		}

		public void Awake() {
			if (_instance == null) {
				_instance = this;
				foreach (Transform child in transform.GetComponentsInChildren<Transform>()) {
					if (child.name.Equals("Crosshair")) {
						Crosshair = child.gameObject;
					}
				}
				DontDestroyOnLoad(this);
			} else {
				Destroy(gameObject);
			}
		}

		public void ShowCrosshair(bool enabled) {
			Crosshair.SetActive(enabled);
		}

		public void FadeToBlack(float duration_s, UIAction callback) {
			//BlackImage.enabled = true;
			//BlackImage.color = Color.black;
			Debug.Log("fade to black!");
		}

	}
}
