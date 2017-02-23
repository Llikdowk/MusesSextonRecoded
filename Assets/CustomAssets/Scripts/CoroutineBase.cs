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

	public void StartCoroutineNoTimescaled(float durationSecs, CoroutineBody coroutineBody, CoroutineCallback callback) {
		StartCoroutine(CoroutineNoTimescaled(durationSecs, coroutineBody, callback));
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


	private IEnumerator CoroutineNoTimescaled(float durationSecs, CoroutineBody coroutineBody, CoroutineCallback callback) {
		float t = 0.0f;
		float time = Time.realtimeSinceStartup;
		while (t < 1.0f) {
			coroutineBody(t);
			float dt = Time.realtimeSinceStartup - time;
			t += dt / durationSecs;
			time = Time.realtimeSinceStartup;
			yield return null;
		}
		if (callback != null) {
			callback();
		}
	}
}
