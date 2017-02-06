
using UnityEngine;

namespace Game.PlayerComponents {
	public class Look : MonoBehaviour {

		[Range(0.0f, 200.0f)] public float XSensitivity = 100.0f;
		[Range(0.0f, 200.0f)] public float YSensitivity = 100.0f;
		[Range(0.0f, 1.0f)] public float VerticalRange = 0.9f;
		public bool Smooth = false;
		private Camera _camera;

		public void Awake() {
			_camera = GetComponentInChildren<Camera>();
		}

		public void Update() {
			const string horizontalAxisName = "Mouse X";
			float axisValue = Smooth ? Input.GetAxis(horizontalAxisName) : Input.GetAxisRaw(horizontalAxisName);
			float speed = axisValue * Time.deltaTime * XSensitivity;
			transform.Rotate(0, speed, 0, Space.Self);


			const string verticalAxisName = "Mouse Y";
			axisValue = Smooth ? Input.GetAxis(verticalAxisName) : Input.GetAxisRaw(verticalAxisName);
			speed = axisValue * Time.deltaTime * YSensitivity;

			_camera.transform.Rotate(-speed, 0, 0, Space.Self);
			float d = Vector3.Dot(_camera.transform.forward, transform.forward);
			if (d < 1 - VerticalRange) {
				_camera.transform.Rotate(speed, 0, 0, Space.Self);
			}
		}

	}
}
