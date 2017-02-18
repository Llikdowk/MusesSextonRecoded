using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public struct VerseInfo {
		public string Verse;
		public PoemState.Gender Gender;
		public string FirstPersonVerse;
	}

	public class VerseInfoComponent : MonoBehaviour {
		public VerseInfo Info;
	}
}