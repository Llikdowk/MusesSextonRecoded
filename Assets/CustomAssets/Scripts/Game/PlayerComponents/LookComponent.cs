
using System;
using UnityEngine;

namespace Game.PlayerComponents {

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
	}


	public class Look {
		private static Transform _superFixedTransform;
		private readonly GameObject _player;
		private LookComponent _lookComponent;

		public Look(GameObject player) {
			_player = player;
			if (!_superFixedTransform) {
				_superFixedTransform = new GameObject("_superFixedTransform").transform;
			}
		}

		public void SetFreeLook(LookConfig config) {
			_lookComponent = _player.GetOrAddComponent<LookComponent>();
			_lookComponent.CurrentConfig = config;
			_lookComponent.FixedForward = _player.transform;
		}

		public void SetScopedLook(LookConfig config, Transform fixedForward) {
			_lookComponent = _player.GetOrAddComponent<LookComponent>();
			_lookComponent.CurrentConfig = config;
			_lookComponent.FixedForward = fixedForward;
		}

		public void SetScopedLook(LookConfig config, Quaternion fixedRotation) {
			_lookComponent = _player.GetOrAddComponent<LookComponent>();
			_lookComponent.CurrentConfig = config;
			_superFixedTransform.rotation = fixedRotation;
			_superFixedTransform.position = _player.transform.position;
			_lookComponent.FixedForward = _superFixedTransform;
		}
	}

	public class LookComponent : MonoBehaviour {

		public LookConfig CurrentConfig;
		private Camera _camera;
		private float _xSpeed;
		private float _ySpeed;
		[HideInInspector]
		public Transform FixedForward;


		public void Start() {
			CurrentConfig = new LookConfig();
			_camera = GetComponentInChildren<Camera>();
			if (FixedForward == null) {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(Transform), "Initialized with <b>this</b> object transform instead.");
				FixedForward = transform;
			}
		}

		public void Update() {
			float xSensitivity = CurrentConfig.XSensitivity * CurrentConfig.SensitivityMultiplicator;
			float ySensitivity = CurrentConfig.YSensitivity * CurrentConfig.SensitivityMultiplicator;

			const string horizontalAxisName = "Mouse X";
			float axisValue = CurrentConfig.SmoothInput ? Input.GetAxis(horizontalAxisName) : Input.GetAxisRaw(horizontalAxisName);
			_xSpeed = Mathf.Lerp(axisValue * Time.deltaTime * xSensitivity, _xSpeed, CurrentConfig.HorizontalLag);

			Quaternion rotation = Quaternion.AngleAxis(_xSpeed, Vector3.up) * transform.rotation;
			float angle = Quaternion.Angle(rotation, FixedForward.rotation);
			if (CurrentConfig.SmoothHorizontalClipping) {
				transform.rotation = Quaternion.Slerp(rotation, transform.rotation,
					CurrentConfig.HorizontalForceBack * angle / CurrentConfig.HorizontalRangeDegrees);
			}
			else {
				if (angle < CurrentConfig.HorizontalRangeDegrees) {
					transform.rotation = Quaternion.Slerp(rotation, transform.rotation, CurrentConfig.HorizontalLag);
				}
			}

			const string verticalAxisName = "Mouse Y";
			axisValue = CurrentConfig.SmoothInput ? Input.GetAxis(verticalAxisName) : Input.GetAxisRaw(verticalAxisName);
			_ySpeed = Mathf.Lerp(axisValue * Time.deltaTime * ySensitivity, _ySpeed, CurrentConfig.VerticalLag);
			rotation = Quaternion.AngleAxis(_ySpeed, Vector3.left) * _camera.transform.localRotation;
			angle = Quaternion.Angle(rotation, Quaternion.AngleAxis(0, transform.forward));
			if (CurrentConfig.SmoothVerticalClipping) {
				_camera.transform.localRotation = Quaternion.Slerp(rotation, _camera.transform.localRotation,
					CurrentConfig.VerticalForceBack * angle / CurrentConfig.VerticalRangeDegrees);
			}
			else {
				if (angle < CurrentConfig.VerticalRangeDegrees) {
					_camera.transform.localRotation = rotation;
				}
			}
		}
	}
}
