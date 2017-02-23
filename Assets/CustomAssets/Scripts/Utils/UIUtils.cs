using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utils {

	// TODO move to UIGameComponent
	public static class UIUtils {
		public delegate void UIAction();

		// Time.timeScale do not affect this
		public static IEnumerator FadeOut(Image img, float duration_s, UIAction f = null) {
			float t = 0.0f;
			float time = Time.realtimeSinceStartup;
			Color original = img.color;
			Color final = img.color;
			final.a = 0.0f;
			while (t < 1.0f) {
				img.color = Color.Lerp(original, final, t);
				float dt = Time.realtimeSinceStartup - time;
				t += dt/duration_s;
				time = Time.realtimeSinceStartup;
				yield return null;
			}
			if (f != null) f();
		}

	}
}