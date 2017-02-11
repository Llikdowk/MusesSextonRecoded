using System.Collections;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

namespace Audio {

	public class AudioController : MonoBehaviour {

		[Range(0, 1)] public float Music1 = 1.0f;
		public float Music1FadeTime = 15.0f;
		[Range(0, 1)] public float Music2 = 1.0f;
		[Range(0, 1)] public float Music3 = 1.0f;
		[Range(0, 1)] public float Percussion = 1.0f;
		[Range(0, 1)] public float Wind = 0.75f;
		public float WindFadeTime = 5.0f;

		[Range(0, 1)] public float Bell = 0.0f;

		[Range(0, 1)] public float StepsSpeed = 0.0f;
		[Range(0, 1)] public float StepsVolume = 0.5f;

		private FMODObject _music;
		private FMODObject _wind;
		private FMODObject _bell;
		private FMODObject _steps;

		private static AudioController _instance;
		public static AudioController GetInstance() { return _instance; }

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(gameObject);
				return;
			}
			_instance = this;

			_music = new FMODObject("event:/Music");
			_wind = new FMODObject("event:/Wind");
			_bell = new FMODObject("event:/Bell");
			_bell.SetParameter("Volume", Bell);
			_steps = new FMODObject("event:/Steps");
		}


		public void Update() {
			_steps.SetParameter("Volume", StepsVolume);
		}

		public void PlaySteps() {
			if (StepsSpeed == 0.0f) return;
			if (_steps.IsPlaying()) {
				if (_steps.GetNormalizedTimelinePosition() >= 1 - StepsSpeed) {
					_steps.Play();
				}
			}
			else {
				_steps.Play();
			}
		}

		public void PlayBell() {
			_bell.Play();
		}

		public void FadeInWind() {
			_wind.Play();
			StartCoroutine(Fade(_wind, "Volume", Wind, WindFadeTime));
		}

		public void FadeInMusic1() {
			_music.Play();
			StartCoroutine(Fade(_music, "Music1", Music1, Music1FadeTime));
		}

		private IEnumerator Fade(FMODObject audio, string paramName, float endVolume, float duration_s) {
			float t = 0.0f;
			float originalVolume = audio.GetParameter(paramName);
			float time = Time.realtimeSinceStartup;
			while (t < 1.0f) {
				audio.SetParameter(paramName, Mathf.Lerp(originalVolume, endVolume, t));
				float dt = Time.realtimeSinceStartup - time;
				t += dt / duration_s;
				time = Time.realtimeSinceStartup;
				yield return null;
			}
		}
	}
}
