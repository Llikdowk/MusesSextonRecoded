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

		public void Awake() {
			string jsonText = System.IO.File.ReadAllText("Assets/Resources/poems.json");
			LandmarkVerses[] verses = JsonConvert.DeserializeObject<LandmarkVerses[]>(jsonText);
			foreach (Transform child in gameObject.GetComponentsInChildren<Transform>()) {
				foreach (LandmarkVerses v in verses) {
					if (child.name.Contains(v.LandmarkName)) {
						child.gameObject.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
						break;
					}
				}
			}
		}

	}
}