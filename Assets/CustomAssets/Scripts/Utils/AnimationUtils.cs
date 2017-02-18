using System.Collections;
using Game.PlayerComponents;
using UnityEngine;

namespace Utils {

	public delegate void AnimationDelegate();

	public static class AnimationUtils {

		public static void SlerpTowards(Transform target, Vector3 destination, float durationSecs, AnimationDelegate callback = null) {
			SlerpForwardComponent c = target.gameObject.AddComponent<SlerpForwardComponent>();
			c.Init(target, destination, durationSecs, callback);
			c.Run();
		}


	}

	internal class SlerpForwardComponent : MonoBehaviour {
		private Transform _target;
		private Vector3 _destination;
		private float _durationSecs;
		private AnimationDelegate _callback;

		public void Init(Transform target, Transform destination, float durationSecs, AnimationDelegate callback) {
			_target = target;
			_destination = destination.forward;
			_durationSecs = durationSecs;
			_callback = callback;
		}

		public void Init(Transform target, Vector3 destination, float durationSecs, AnimationDelegate callback) {
			_target = target;
			_destination = destination;
			_durationSecs = durationSecs;
			_callback = callback;
		}

		public void Run() {
			if (_durationSecs > 0.0f) {
				StartCoroutine(ApplySlerp());
			}
		}

		public IEnumerator ApplySlerp() {
			float t = 0.0f;
			Vector3 originalForward = _target.forward;
			while (t < 1.0f) {
				_target.forward = Vector3.Slerp(originalForward, _destination, t);
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
