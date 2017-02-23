using UnityEngine;

namespace Utils {


	public class AnimationUtils {

		public static void LookTowardsHorizontal(Transform target, Vector3 newHorizontalLookDir, float durationSecs, 
			CoroutineBase.CoroutineCallback callback = null) 
		{
			LookTowardsHorizontalComponent anim = target.gameObject.AddComponent<LookTowardsHorizontalComponent>();
			anim.Init(newHorizontalLookDir, durationSecs, callback);
			anim.Run();
		}

		public static void LookTowardsVertical(Transform target, Vector3 focusPoint, float durationSecs,
			CoroutineBase.CoroutineCallback callback = null) 
		{
			LookTowardsVerticalComponent anim = target.gameObject.AddComponent<LookTowardsVerticalComponent>();
			anim.Init(focusPoint, durationSecs, callback);
			anim.Run();
		}

		public static void MoveSmoothlyTo(Transform target, Vector3 destination, float durationSecs,
			CoroutineBase.CoroutineCallback callback = null) 
		{
			MoveSmoothlyAnimationComponent anim = target.gameObject.AddComponent<MoveSmoothlyAnimationComponent>();
			anim.Init(destination, durationSecs, callback);
			anim.Run();
		}

		private interface IAnimationUtils {
			void Init(Vector3 v, float f, CoroutineBase.CoroutineCallback c);
			void Run();
		}

		private class LookTowardsHorizontalComponent : CoroutineBase, IAnimationUtils {
			private Quaternion _horizontalRotation;
			private float _durationSecs;
			private CoroutineCallback _callback;

			public void Init(Vector3 newHorizontalLookDir, float durationSecs, CoroutineCallback callback = null) {
				_horizontalRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(newHorizontalLookDir, transform.up), transform.up);
				_durationSecs = durationSecs;
				_callback = callback;
			}

			public void Run() {
				Transform horizontal = transform;
				Quaternion startHorizontal = horizontal.rotation;
				StartVolatileCoroutine(_durationSecs, 
					(t) => {
						horizontal.rotation = Quaternion.Slerp(startHorizontal, _horizontalRotation, t);
					},
					_callback
				);
			}
		}

		private class LookTowardsVerticalComponent : CoroutineBase, IAnimationUtils {
			private float _angle = 0.0f;
			private float _durationSecs;
			private CoroutineCallback _callback;

			public void Init(Vector3 focusPoint, float durationSecs, CoroutineCallback callback = null) {
				Vector3 lookDir = (focusPoint - transform.position).normalized;
				float sign = -1;
				if (transform.forward.y > lookDir.y) {
					sign = 1;
				}
				lookDir = new Vector3(transform.forward.x, lookDir.y, transform.forward.z);
				_angle = Vector3.Angle(transform.forward, lookDir) * sign;
				_durationSecs = durationSecs;
				_callback = callback;
			}

			public void Run() {
				StartVolatileCoroutine(_durationSecs,
					(t) => {
						float dt = Time.deltaTime / _durationSecs;
						float angleStep = dt * _angle;
						transform.rotation *= Quaternion.AngleAxis(angleStep, Vector3.right);
					},
					_callback
				);
				
			}
		}

		private class MoveSmoothlyAnimationComponent : CoroutineBase, IAnimationUtils {
			private Vector3 _start;
			private Vector3 _destination;
			private float _durationSecs;
			private CoroutineCallback _callback;

			public void Init(Vector3 destination, float durationSecs, CoroutineCallback callback = null) {
				_start = transform.position;
				_destination = destination;
				_durationSecs = durationSecs;
				_callback = callback;
			}

			public void Run() {
				StartVolatileCoroutine(_durationSecs,
					(t) => {
						transform.position = Vector3.Lerp(_start, _destination, t);
					},
					_callback
				);
			}
		}
	}

}