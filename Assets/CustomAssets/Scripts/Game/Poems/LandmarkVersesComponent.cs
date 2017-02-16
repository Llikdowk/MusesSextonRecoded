using System;
using UnityEngine;

namespace Game.Poems {

	[Serializable]
	public struct LandmarkVerses {
		public string LandmarkName;
		public string[] Masculine;
		public string[] Feminine;
		public string[] Plural;
		public string[] FirstPerson;
	}

	public class LandmarkVersesComponent : MonoBehaviour {
		public LandmarkVerses LandmarkVerses;
	}
}
