
using System;
using UnityEngine;

namespace Game.PlayerComponents {

	public class Look : MonoBehaviour {

		[Serializable]
		public class LookConfig {
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
		}

		[HideInInspector]
		public LookConfig Config = new LookConfig();
		private Camera _camera;
		private float _xSpeed;
		private float _ySpeed;


		public void Awake() {
			_camera = GetComponentInChildren<Camera>();
		}

		public void Start() {
			if (Config.FixedForward == null) {
				Debug.LogWarning("Not Fixed Forward found: initialized with this object transform!");
				Config.FixedForward = transform;
			}
		}

		public void Update() {
			float xSensitivity = Config.XSensitivity * Config.SensitivityMultiplicator;
			float ySensitivity = Config.YSensitivity * Config.SensitivityMultiplicator;

			const string horizontalAxisName = "Mouse X";
			float axisValue = Config.SmoothInput ? Input.GetAxis(horizontalAxisName) : Input.GetAxisRaw(horizontalAxisName);
			_xSpeed = Mathf.Lerp(axisValue * Time.deltaTime * xSensitivity, _xSpeed, Config.HorizontalLag);

			Quaternion rotation = Quaternion.AngleAxis(_xSpeed, Vector3.up) * transform.rotation;
			float angle = Quaternion.Angle(rotation, Config.FixedForward.rotation);
			if (Config.SmoothHorizontalClipping) {
				transform.rotation = Quaternion.Slerp(rotation, transform.rotation,
					Config.HorizontalForceBack * angle / Config.HorizontalRangeDegrees);
			}
			else {
				if (angle < Config.HorizontalRangeDegrees) {
					transform.rotation = Quaternion.Slerp(rotation, transform.rotation, Config.HorizontalLag);
				}
			}

			const string verticalAxisName = "Mouse Y";
			axisValue = Config.SmoothInput ? Input.GetAxis(verticalAxisName) : Input.GetAxisRaw(verticalAxisName);
			_ySpeed = Mathf.Lerp(axisValue * Time.deltaTime * ySensitivity, _ySpeed, Config.VerticalLag);
			rotation = Quaternion.AngleAxis(_ySpeed, Vector3.left) * _camera.transform.localRotation;
			angle = Quaternion.Angle(rotation, Quaternion.AngleAxis(0, transform.forward));
			if (Config.SmoothVerticalClipping) {
				_camera.transform.localRotation = Quaternion.Slerp(rotation, _camera.transform.localRotation,
					Config.VerticalForceBack * angle / Config.VerticalRangeDegrees);
			}
			else {
				if (angle < Config.VerticalRangeDegrees) {
					_camera.transform.localRotation = rotation;
				}
			}
		}
	}
}
