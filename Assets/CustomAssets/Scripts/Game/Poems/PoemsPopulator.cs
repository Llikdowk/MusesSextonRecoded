using Newtonsoft.Json;
using UnityEngine;

namespace Game.Poems {


	public class PoemsPopulator : MonoBehaviour {

		public GameObject[] OtherLandmarks;
		// dynamic:
		//     cart
		//     coffin
		//     other grave
		// 
		// not 3d-modelled:
		//     vulture
		//     well

		
		private struct LandmarkPoem {
			public string LandmarkName;
			public string[] Masculine;
			public string[] Feminine;
			public string[] Plural;
			public string[] FirstPerson;
		}

		public void Start() {
			string jsonText = System.IO.File.ReadAllText("Assets/Resources/poems.json");
			LandmarkPoem[] test = JsonConvert.DeserializeObject<LandmarkPoem[]>(jsonText);
		}

	}
}