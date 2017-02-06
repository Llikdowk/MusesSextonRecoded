
using UnityEngine;

namespace Game.PlayerComponents {
	public class Look : MonoBehaviour {

		[Range(0, 1)] public float XSensitivity = 0.5f;
		[Range(0, 1)] public float YSensitivity = 0.5f;
		[Range(0, 90)] public float VerticalRangeDegrees = 90.0f;

		public bool Smooth = false;
		private Camera _camera;

		public void Awake() {
			_camera = GetComponentInChildren<Camera>();
		}

		public void Update() {
			float xSensitivity = XSensitivity * 200.0f;
			float ySensitivity = YSensitivity * 200.0f;

			const string horizontalAxisName = "Mouse X";
			float axisValue = Smooth ? Input.GetAxis(horizontalAxisName) : Input.GetAxisRaw(horizontalAxisName);
			float speed = axisValue * Time.deltaTime * xSensitivity;
			transform.Rotate(0, speed, 0, Space.Self);
			//TODO horizontal clip

			const string verticalAxisName = "Mouse Y";
			axisValue = Smooth ? Input.GetAxis(verticalAxisName) : Input.GetAxisRaw(verticalAxisName);
			speed = axisValue * Time.deltaTime * ySensitivity;

			Quaternion rotation = Quaternion.AngleAxis(speed, Vector3.left) * _camera.transform.localRotation;
			float angle = rotation.eulerAngles.x;
			if ((angle < VerticalRangeDegrees || angle > 360.0f - VerticalRangeDegrees) && rotation.eulerAngles.y < 179.0f) {
				_camera.transform.localRotation = rotation;
			}
		}
	}
}
