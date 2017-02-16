using Newtonsoft.Json;
using UnityEngine;

namespace Game.Poems {


	public class VersesPopulatorComponent : MonoBehaviour {

		public GameObject[] OtherLandmarks;
		// dynamic:
		//     cart
		//     coffin
		//     other grave
		// 
		// not 3d-modelled:
		//     vulture
		//     well

		

		public void Start() {
			string jsonText = System.IO.File.ReadAllText("Assets/Resources/poems.json");
			LandmarkVerses[] test = JsonConvert.DeserializeObject<LandmarkVerses[]>(jsonText);
		}

	}
}