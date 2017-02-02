
using UnityEngine;

namespace Scripts.Game {
	public class GlobalConfig : MonoBehaviour {

		public void Awake() {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

	}
}
