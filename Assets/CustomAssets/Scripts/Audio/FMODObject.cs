using System;
using FMOD.Studio;

namespace Audio {
	public class FMODObject {

		private readonly FMOD.Studio.EventInstance _audio;
		private readonly FMOD.Studio.EventDescription _info;

		public FMODObject(string eventName) {
			_audio = FMODUnity.RuntimeManager.CreateInstance(eventName);
			_audio.getDescription(out _info);
			_info.loadSampleData();
		}

		/// <summary>
		/// Returns current timeline position in seconds
		/// </summary>
		public float GetTimelinePosition() {
			int current_ms;
			_audio.getTimelinePosition(out current_ms);
			return current_ms / 1000.0f;
		}

		/// <summary>
		/// Returns normalized current timeline position (from 0 to 1)
		/// </summary>
		/// <returns></returns>
		public float GetNormalizedTimelinePosition() {
			int current_ms;
			_audio.getTimelinePosition(out current_ms);

			int length_ms;
			_info.getLength(out length_ms);

			return current_ms / (float) length_ms;
		}

		public void Play() {
			StopImmediate();
			_audio.start();
		}

		public void Mute() {
			_audio.setVolume(0.0f);
		}

		public void Unmute() {
			_audio.setVolume(1.0f);
		}

		public void StopFading() {
			_audio.stop(STOP_MODE.ALLOWFADEOUT);
		}

		public void StopImmediate() {
			_audio.stop(STOP_MODE.IMMEDIATE);
		}

		public void Release() {
			_audio.release();
		}

		public void SetParameter(string paramName, float value) {
			FMOD.RESULT r = _audio.setParameterValue(paramName, value);
			if (r != FMOD.RESULT.OK) {
				throw new NullReferenceException("Audio parameter <" + paramName + "> not found in audio");
			}
		}

		public float GetParameter(string paramName) {
			ParameterInstance p;
			_audio.getParameter(paramName, out p);
			float value;
			p.getValue(out value);
			return value;
		}

		public bool IsPlaying() {
			PLAYBACK_STATE state;
			_audio.getPlaybackState(out state);
			return state == PLAYBACK_STATE.PLAYING;
		}
	}
}
