
using System.Collections;
using UnityEngine;

namespace Game.Entities {
	public class GiantDoorComponent : MonoBehaviour {

		public float Duration_s = 4.5f;
		private GameObject _leftDoor;
		private GameObject _rightDoor;

		public void Awake() {
			_leftDoor = GameObject.Find("LeftDoor").gameObject;
			_rightDoor = GameObject.Find("RightDoor").gameObject;
		}


		public void Open() {
			StartCoroutine(DoOpen());
		}


		private IEnumerator DoOpen() {
			float t = 0.0f;
			while (t < 1.0f) {
				t += Time.deltaTime / Duration_s;
				_leftDoor.transform.Rotate(Vector3.up, 0.1f);
				_rightDoor.transform.Rotate(Vector3.up, -0.1f);
				yield return null;
			}
		}

	}
}
