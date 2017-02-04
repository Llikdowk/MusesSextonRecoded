using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utils {
	public static IEnumerator FadeOut(Image img, float speed) {
		const float dt = 0.033f;
		float t = 0.0f;
		Color original = img.color;
		Color final = img.color;
		final.a = 0.0f;
		while (t < 1.0f) {
			img.color = Color.Lerp(original, final, t);
			t += dt * speed;
			if (Time.timeScale == 0.0f) { // TODO: find a workaround for this hack
				yield return null; //new WaitForSeconds(dt);
			}
			else {
				yield return new WaitForEndOfFrame();
			}
		}
	}

}
