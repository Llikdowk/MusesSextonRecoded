using Newtonsoft.Json;
using UnityEngine;

namespace Game.Poems {


	public struct LandmarkPoem {
		public string LandmarkName;
		public string[] Masculine;
		public string[] Feminine;
		public string[] Plural;
		public string[] FirstPerson;
	}

	public class PoemsPopulator : MonoBehaviour {

		public void Start() {
			string jsonText = System.IO.File.ReadAllText("Assets/Resources/poems.json");
			LandmarkPoem[] test = JsonConvert.DeserializeObject<LandmarkPoem[]>(jsonText);

		}

	}
}