
using System.Net;
using UnityEngine;

namespace Game.CustomInput {
	public class LookVertical : MonoBehaviour {

		[Range(0.0f, 200.0f)] public float Sensitivity = 100.0f;
		[Range(0.0f, 1.0f)] public float VerticalRange = 0.9f;

		private Camera _camera;

		public void Awake() {
			_camera = GetComponentInChildren<Camera>();
		}

		public void Update() {
			float speed = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * Sensitivity;

			_camera.transform.Rotate(-speed, 0, 0, Space.Self);
			float d = Vector3.Dot(_camera.transform.forward, transform.forward);
			if (d < 1 - VerticalRange) {
				_camera.transform.Rotate(speed, 0, 0, Space.Self); // TODO clamp angle
			}
		}

	}
}
