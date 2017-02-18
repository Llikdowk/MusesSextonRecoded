using UnityEngine;
using Utils;

namespace Game.Entities {
	class ShovelMovementComponent : MonoBehaviour {

		public float Speed = 5.0f;
		public float Clamp = 0.05f;
		[Range(0, 1)] public float VerticalLag = 0.75f;
		private float dt = 0.0f;
		private Vector3 _originalPosition;
		private Vector3 _lastPosition;
		private bool _hidden = false;
		private float _hiddenDistance = 2.5f;
		private MeshRenderer _renderer;

		public void Awake() {
			_renderer = GetComponent<MeshRenderer>();
		}


		public void Start() {
			_originalPosition = transform.localPosition;
		}

		public void Update() {
			if (!_hidden) {
				float dposSqr = (transform.parent.position - _lastPosition).sqrMagnitude;
				_lastPosition = transform.parent.position;
				if (dposSqr > 0.0f) {
					transform.localPosition = _originalPosition + Vector3.up * Clamp * Mathf.Sin(dt * Speed);
					dt += Time.deltaTime;
				}
			}
		}

		public void Hide() {
			if (_hidden) return;
			_hidden = true;
			AnimationUtils.MoveSmoothlyTo(transform, transform.position, 
				transform.position-transform.up*_hiddenDistance, 0.25f, () => {
					_renderer.enabled = false;
				});
			_renderer.enabled = false;

		}

		public void Show() {
			if (!_hidden) return;
			_renderer.enabled = true;
			transform.LocalReset();
			_hidden = false;
			/*
			AnimationUtils.MoveSmoothlyTo(transform, transform.position, 
				transform.position+transform.up*_hiddenDistance, 0.25f, () => {
					_hidden = false;
				});
			*/
		}

	}
}
