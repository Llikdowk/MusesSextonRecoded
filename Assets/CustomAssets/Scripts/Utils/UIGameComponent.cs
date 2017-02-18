
using UnityEngine;
using UnityEngine.UI;

namespace Utils {

	public class UIGameComponent : MonoBehaviour {
		private static UIGameComponent _instance = null;
		public GameObject Crosshair;

		public static UIGameComponent GetInstance() {
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
	}
}
