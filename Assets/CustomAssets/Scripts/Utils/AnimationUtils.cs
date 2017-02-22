using System.Collections;
using Game.PlayerComponents;
using UnityEngine;

namespace Utils {

	public delegate void AnimationDelegate();

	public class AnimationUtils {

		public static void SlerpTowards(Transform target, Vector3 newHorizontalLookDir, float durationSecs, AnimationDelegate callback = null) {
			SlerpAnimationComponent anim = target.gameObject.AddComponent<SlerpAnimationComponent>();
			anim.Init(newHorizontalLookDir, durationSecs, callback);
			anim.Run();
		}

		public static void MoveSmoothlyTo(Transform target, Vector3 start, Vector3 destination, float durationSecs,
			AnimationDelegate callback = null) {
			MoveSmoothlyAnimationComponent anim = target.gameObject.AddComponent<MoveSmoothlyAnimationComponent>();
			anim.Init(start, destination, durationSecs);
			anim.Run();
		}


		private abstract class AnimationHelper : MonoBehaviour {
			private float _durationSecs;
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

		private class SlerpAnimationComponent : AnimationHelper {
			private Quaternion _horizontalRotation;

			public void Init(Vector3 newHorizontalLookDir, float durationSecs, AnimationDelegate callback = null) {
				base.Init(durationSecs, callback);
				_horizontalRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(newHorizontalLookDir, transform.up), transform.up);
			}

			public override void Run() {
				Transform horizontal = transform;
				Transform vertical = Player.GetInstance().Camera.transform;
				Quaternion startHorizontal = horizontal.rotation;

				ApplyCoroutine(() => {
					horizontal.rotation = Quaternion.Slerp(startHorizontal, _horizontalRotation, t);
					vertical.rotation *= Quaternion.AngleAxis(0.5f, Vector3.right);
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