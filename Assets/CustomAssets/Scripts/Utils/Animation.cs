using System.Collections;
using UnityEngine;

namespace Utils {

	public delegate void AnimationDelegate();

	public static class Animation {

		public static void SlerpTowards(Transform target, Transform destination, float durationSecs, AnimationDelegate callback) {
			SlerpForwardComponent c = target.gameObject.AddComponent<SlerpForwardComponent>();
			c.Init(target, destination, durationSecs, callback);
			c.Run();
		}

	}

	internal class SlerpForwardComponent : MonoBehaviour {
		private Transform _target;
		private Transform _destination;
		private float _durationSecs;
		private AnimationDelegate _callback;

		public void Init(Transform target, Transform destiny, float durationSecs, AnimationDelegate callback) {
			_target = target;
			_destination = destiny;
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
				_target.forward = Vector3.Slerp(originalForward, _destination.forward, t);
				t += Time.deltaTime / _durationSecs;
				yield return null;
			}
			_callback();
			Destroy(this);
		}
	

}
}
