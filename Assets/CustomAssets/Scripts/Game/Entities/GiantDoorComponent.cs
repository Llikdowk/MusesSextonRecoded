
using System.Collections;
using Audio;
using FMODUnity;
using Game.PlayerComponents;
using UnityEngine;

namespace Game.Entities {
	public class GiantDoorComponent : MonoBehaviour {

		private float duration_s = 7.5f;
		private GameObject _leftDoor;
		private GameObject _rightDoor;
		private StudioEventEmitter _emitter;

		public void Awake() {
			_leftDoor = GameObject.Find("LeftDoor").gameObject;
			_rightDoor = GameObject.Find("RightDoor").gameObject;
			_emitter = GetComponentInChildren<StudioEventEmitter>();
		}


		public void Open() {
			_emitter.enabled = true;
			AudioController.GetInstance().PlayDoor();
			Player.GetInstance().CameraController.Shake(duration_s);
			StartCoroutine(DoOpen());
		}


		private IEnumerator DoOpen() {
			float t = 0.0f;
			while (t < 1.0f) {
				t += Time.deltaTime / duration_s;
				_leftDoor.transform.Rotate(Vector3.up, 0.075f);
				_rightDoor.transform.Rotate(Vector3.up, -0.075f);
				yield return null;
			}
		}

	}
}
