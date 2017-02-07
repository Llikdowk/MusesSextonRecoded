
using UnityEngine;

namespace Game.PlayerComponents {
	public class Look : MonoBehaviour {

		public bool SmoothInput = false;
		[Range(0, 1)] public float XSensitivity = 0.5f;
		[Range(0, 1)] public float YSensitivity = 0.5f;
		public float SensitivityMultiplicator = 200.0f;

		public bool SmoothVerticalClipping = false;
		[Range(0, 90)] public float VerticalRangeDegrees = 90.0f;
		[Range(0, 1)] public float VerticalForceBack = 0.5f;
		[Range(0, 1)] public float VerticalLag = 0.0f;

		public bool SmoothHorizontalClipping = false;
		[Range(0, 181)] public float HorizontalRangeDegrees = 181.0f;
		[Range(0, 1)] public float HorizontalForceBack = 0.5f;
		[Range(0, 1)] public float HorizontalLag = 0.0f;
		public Transform FixedForward;

		private Camera _camera;
		private float _xSpeed;
		private float _ySpeed;


		public void Awake() {
			_camera = GetComponentInChildren<Camera>();
		}

		public void Start() {
			if (FixedForward == null) {
				Debug.LogWarning("Not Fixed Forward found: initialized with this object transform!");
				FixedForward = transform;
			}
		}

		public void Update() {
			float xSensitivity = XSensitivity * SensitivityMultiplicator;
			float ySensitivity = YSensitivity * SensitivityMultiplicator;

			const string horizontalAxisName = "Mouse X";
			float axisValue = SmoothInput ? Input.GetAxis(horizontalAxisName) : Input.GetAxisRaw(horizontalAxisName);
			_xSpeed = Mathf.Lerp(axisValue * Time.deltaTime * xSensitivity, _xSpeed, HorizontalLag);

			Quaternion rotation = Quaternion.AngleAxis(_xSpeed, Vector3.up) * transform.rotation;
			float angle = Quaternion.Angle(rotation, FixedForward.rotation);
			if (SmoothHorizontalClipping) {
				transform.rotation = Quaternion.Slerp(rotation, transform.rotation,
					HorizontalForceBack * angle / HorizontalRangeDegrees);
			}
			else {
				if (angle < HorizontalRangeDegrees) {
					transform.rotation = Quaternion.Slerp(rotation, transform.rotation, HorizontalLag);
				}
			}

			const string verticalAxisName = "Mouse Y";
			axisValue = SmoothInput ? Input.GetAxis(verticalAxisName) : Input.GetAxisRaw(verticalAxisName);
			_ySpeed = Mathf.Lerp(axisValue * Time.deltaTime * ySensitivity, _ySpeed, VerticalLag);
			rotation = Quaternion.AngleAxis(_ySpeed, Vector3.left) * _camera.transform.localRotation;
			angle = Quaternion.Angle(rotation, Quaternion.AngleAxis(0, transform.forward));
			if (SmoothVerticalClipping) {
				_camera.transform.localRotation = Quaternion.Slerp(rotation, _camera.transform.localRotation,
					VerticalForceBack * angle / VerticalRangeDegrees);
			}
			else {
				if (angle < VerticalRangeDegrees) {
					_camera.transform.localRotation = rotation;
				}
			}
		}
	}
}
