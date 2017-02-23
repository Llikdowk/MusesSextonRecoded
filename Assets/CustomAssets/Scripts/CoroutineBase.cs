using System.Collections;
using UnityEngine;

public abstract class CoroutineBase : MonoBehaviour {
	public delegate void CoroutineBody(float f);
	public delegate void CoroutineCallback();

	public void StartCoroutine(float durationSecs, CoroutineBody coroutineBody, CoroutineCallback callback) {
		StartCoroutine(Coroutine(durationSecs, coroutineBody, callback));
	}

	public void StartVolatileCoroutine(float durationSecs, CoroutineBody coroutineBody, CoroutineCallback callback) {
		CoroutineCallback autoDestruction = ()=>Object.Destroy(this);
		if (callback != null) {
			callback += autoDestruction;
		}
		else {
			callback = autoDestruction;
		}

		StartCoroutine(Coroutine(durationSecs, coroutineBody, callback));
	}

	private IEnumerator Coroutine(float durationSecs, CoroutineBody coroutineBody, CoroutineCallback callback) {
		float t = 0.0f;
		while (t < 1.0f) {
			coroutineBody(t);
			t += Time.deltaTime / durationSecs;
			yield return null;
		}
		if (callback != null) {
			callback();
		}
	}
}
