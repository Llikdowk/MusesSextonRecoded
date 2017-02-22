
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Game.CameraComponents {
	public class CameraController : MonoBehaviour {
		private Camera _main;
		private Camera _landmark;
		private UnsaturatePostEffect _saturation;
		private OutlinerPostEffect _outliner;
		private DepthOfField _depthOfField;
		private CameraShake _shake;

		private delegate void CoroutineAction(float t);
		private delegate void CoroutineCallback();

		public void Init(Camera mainCamera, Camera landmarkCamera) {
			_main = mainCamera;
			_landmark = landmarkCamera;
			_saturation = _main.gameObject.GetComponent<UnsaturatePostEffect>();
			if (!_saturation) {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(UnsaturatePostEffect));
			}
			_outliner = _landmark.gameObject.GetComponent<OutlinerPostEffect>();
			if (!_outliner) {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(OutlinerPostEffect));
			}
			_depthOfField = _landmark.gameObject.GetComponent<DepthOfField>();
			if (!_depthOfField) {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(DepthOfField));
			}
			else {
				_depthOfField.enabled = false;
			}
			_shake = _main.GetComponent<CameraShake>();
			if (!_shake) {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(CameraShake));
			}
			
		}

		public void Unsaturate(float endValue, float duration_s) {
			float startValue = _saturation.Intensity;
			endValue = Mathf.Min(endValue, 1.0f);
			StartCoroutine(GenericCoroutine(
				(t) => { _saturation.Intensity = Mathf.Lerp(startValue, endValue, t); },
				duration_s	
			));
		}

		public void EnableDepthOfField(float duration_s) {
			_outliner.enabled = false; // collides with dop effect
			_depthOfField.enabled = true;
			float dopStartAperture = _depthOfField.aperture;
			float dopStartFocalSize = _depthOfField.focalSize;
			StartCoroutine(GenericCoroutine(
				(t) => {
					_depthOfField.aperture = Mathf.Lerp(dopStartAperture, 0.759f, t);
					_depthOfField.focalSize = Mathf.Lerp(dopStartFocalSize, 0.18f, t);
				},
				duration_s
			));

		}

		public void DisableDepthOfField(float duration_s) {
			if (!_depthOfField.enabled) return;
			float dopStartAperture = _depthOfField.aperture;
			float dopStartFocalSize = _depthOfField.focalSize;
			StartCoroutine(GenericCoroutine(
				(t) => {
					_depthOfField.aperture = Mathf.Lerp(dopStartAperture, 0.0f, t);
					_depthOfField.focalSize = Mathf.Lerp(dopStartFocalSize, 0.0f, t);
				},
				duration_s,
				() => {
					_outliner.enabled = true;
					_depthOfField.enabled = false;
				}
			));
		}

		private IEnumerator GenericCoroutine(CoroutineAction f, float duration_s, CoroutineCallback callback = null) {
			float t = 0.0f;
			while (t < 1.0f) {
				t += Time.deltaTime / duration_s;
				f(t);
				yield return null;
			}

			if (callback != null) {
				callback();
			}
		}

	}
}
