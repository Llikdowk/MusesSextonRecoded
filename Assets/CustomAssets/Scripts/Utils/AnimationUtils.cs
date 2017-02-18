using System.Collections;
using Game.PlayerComponents;
using UnityEngine;

namespace Utils {

	public delegate void AnimationDelegate();

	public static class AnimationUtils {

		public static void SlerpTowards(Transform target, Vector3 start, Vector3 destination, float durationSecs, AnimationDelegate callback = null) {
			AnimationTempComponent c = target.gameObject.AddComponent<AnimationTempComponent>();
			c.Init(target, start, destination, durationSecs, callback);
			c.RunSlepTowards();
		}

		public static void MoveSmoothlyTo(Transform target, Vector3 start, Vector3 destination, float durationSecs,
			AnimationDelegate callback = null) {
			AnimationTempComponent c = target.gameObject.AddComponent<AnimationTempComponent>();
			c.Init(target, start, destination, durationSecs, callback);
			c.RunMoveSmoothlyTo();
		}

	}

	internal class AnimationTempComponent : MonoBehaviour {
		private Transform _target;
		private Vector3 _destination;
		private Vector3 _start;
		private float _durationSecs;
		private AnimationDelegate _callback;

		public void Init(Transform target, Vector3 start, Vector3 destination, float durationSecs, AnimationDelegate callback) {
			_target = target;
			_start = start;
			_destination = destination;
			_durationSecs = durationSecs;
			_callback = callback;
		}

		public void RunSlepTowards() {
			if (_durationSecs > 0.0f) {
				StartCoroutine(ApplySlerp());
			}
		}

		public IEnumerator ApplySlerp() {
			float t = 0.0f;
			//Vector3 originalForward = _target.forward;
			while (t < 1.0f) {
				_target.forward = Vector3.Slerp(_start, _destination, t);
				t += Time.deltaTime / _durationSecs;
				yield return null;
			}
			if (_callback != null) {
				_callback();
			}
			Destroy(this);
		}


		public void RunMoveSmoothlyTo() {
			if (_durationSecs > 0.0f) {
				StartCoroutine(ApplyMoveSmoothly());
			}
		}

		public IEnumerator ApplyMoveSmoothly() {
			float t = 0.0f;
			while (t < 1.0f) {
				_target.position = Vector3.Lerp(_start, _destination, t);
				t += Time.deltaTime / _durationSecs;
				yield return null;
			}
			if (_callback != null) {
				_callback();
			}
			Destroy(this);
		}
	}
}
