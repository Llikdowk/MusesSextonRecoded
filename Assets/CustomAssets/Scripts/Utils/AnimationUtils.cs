using System.Collections;
using UnityEngine;

namespace Utils {

	public delegate void AnimationDelegate();

	public class AnimationUtils {

		public static void LookTowardsHorizontal(Transform target, Vector3 newHorizontalLookDir, float durationSecs, AnimationDelegate callback = null) {
			LookTowardsHorizontalComponent anim = target.gameObject.AddComponent<LookTowardsHorizontalComponent>();
			anim.Init(newHorizontalLookDir, durationSecs, callback);
			anim.Run();
		}

		public static void LookTowardsVertical(Transform target, Vector3 lookAt, float durationSecs,
			AnimationDelegate callback = null) {
			LookTowardsVerticalComponent anim = target.gameObject.AddComponent<LookTowardsVerticalComponent>();
			anim.Init(lookAt, durationSecs, callback);
			anim.Run();
		}

		public static void MoveSmoothlyTo(Transform target, Vector3 start, Vector3 destination, float durationSecs,
			AnimationDelegate callback = null) {
			MoveSmoothlyAnimationComponent anim = target.gameObject.AddComponent<MoveSmoothlyAnimationComponent>();
			anim.Init(start, destination, durationSecs);
			anim.Run();
		}


		private abstract class AnimationHelper : MonoBehaviour {
			protected float _durationSecs;
			private AnimationDelegate _callback;
			protected float t = 0.0f;

			protected virtual void Init(float durationSecs, AnimationDelegate callback = null) {
				_durationSecs = durationSecs;
				_callback = callback;
			}

			public abstract void Run();

			protected void ApplyCoroutine(AnimationDelegate coroutineBody) {
				StartCoroutine(Coroutine(coroutineBody));
			}

			private IEnumerator Coroutine(AnimationDelegate coroutineBody) {
				t = 0.0f;
				while (t < 1.0f) {
					coroutineBody();
					t += Time.deltaTime / _durationSecs;
					yield return null;
				}
				if (_callback != null) {
					_callback();
				}
				Destroy(this);
			}
		}

		private class LookTowardsHorizontalComponent : AnimationHelper {
			private Quaternion _horizontalRotation;

			public void Init(Vector3 newHorizontalLookDir, float durationSecs, AnimationDelegate callback = null) {
				base.Init(durationSecs, callback);
				_horizontalRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(newHorizontalLookDir, transform.up), transform.up);
			}

			public override void Run() {
				Transform horizontal = transform;
				Quaternion startHorizontal = horizontal.rotation;
				ApplyCoroutine(() => {
					horizontal.rotation = Quaternion.Slerp(startHorizontal, _horizontalRotation, t);
				});
			}
		}

		private class LookTowardsVerticalComponent : AnimationHelper {
			private float _angle = 0.0f;

			public void Init(Vector3 lookAt, float durationSecs, AnimationDelegate callback = null) {
				base.Init(durationSecs, callback);
				Vector3 lookDir = lookAt - transform.position;
				lookDir = new Vector3(transform.forward.x, lookDir.y, transform.forward.z);
				float sign = Mathf.Sign(lookDir.y + transform.forward.y);
				_angle = Vector3.Angle(transform.forward, lookDir) * sign;
			}

			public override void Run() {
				Debug.Log(_angle);
				ApplyCoroutine(() => {
					float angleStep = Time.deltaTime / _durationSecs * _angle;
					transform.rotation *= Quaternion.AngleAxis(angleStep, Vector3.right);
				});
				
			}
		}

		private class MoveSmoothlyAnimationComponent : AnimationHelper {
			private Vector3 _start;
			private Vector3 _destination;

			public void Init(Vector3 start, Vector3 destination, float durationSecs, AnimationDelegate callback = null) {
				base.Init(durationSecs, callback);
				_start = start;
				_destination = destination;
			}

			public override void Run() {
				ApplyCoroutine(() => {
					transform.position = Vector3.Lerp(_start, _destination, t);
				});
			}
		}
	}

}