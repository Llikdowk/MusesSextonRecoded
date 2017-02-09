
using System.Collections;
using UnityEngine;

namespace Game.CameraComponents {
	public class CameraShake : MonoBehaviour {
		public AnimationCurve PowerTimeCurve;
		[Range(0, 1)] public float Power = 1.0f;

		public float DurationSeconds;
		public float ForceCloseSpeedMultiplier = 5.0f;

		public float HeightScale = 1.0f;
		public float HeightSpeed = 1.0f;

		public float WidthScale = 1.0f;
		public float WidthSpeed = 1.0f;

		private float _forceCloseSpeed;

		public void OnEnable() {
			_forceCloseSpeed = 1.0f;
			if (DurationSeconds > 0.0f) {
				StartCoroutine(Run());
			}
		}

		public void OnDisable() {
			_forceCloseSpeed = ForceCloseSpeedMultiplier;
		}

		public IEnumerator Run() {
			float t = 0.0f;
			while (t < 1.0f) {
				Power = PowerTimeCurve.Evaluate(t);
				float px = Mathf.PerlinNoise(0.0f, Time.time * WidthSpeed) * 2.0f - 1.0f;
				float py = Mathf.PerlinNoise(Time.time * HeightSpeed, 0.0f) * 2.0f - 1.0f;
				float width = Power * WidthScale * px;
				float height = Power * HeightScale * py;
				transform.localPosition = new Vector3(width, height, 0);
				t += Time.deltaTime / DurationSeconds * _forceCloseSpeed;
				yield return null;
			}
			enabled = false;
		}
	}
}
