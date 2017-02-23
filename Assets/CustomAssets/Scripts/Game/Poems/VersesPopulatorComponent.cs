using Newtonsoft.Json;
using UnityEngine;

namespace Game.Poems {


	public class VersesPopulatorComponent : MonoBehaviour {

		public GameObject CartLandmark;
		public GameObject Coffin0;
		public GameObject Coffin1;
		public GameObject Coffin2;
		// dynamic:
		//     cart
		//     coffin
		//     other grave
		// 
		// not 3d-modelled:
		//     vulture
		//     well

		public void Awake() {
			TextAsset jsonText = Resources.Load<TextAsset>("poems");
			LandmarkVerses[] verses = JsonConvert.DeserializeObject<LandmarkVerses[]>(jsonText.text);
			foreach (Transform child in gameObject.GetComponentsInChildren<Transform>()) {
				foreach (LandmarkVerses v in verses) {
					if (child.name.Contains(v.LandmarkName)) {
						child.gameObject.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
						break;
					}
				}
			}

			if (!CartLandmark) {
				DebugMsg.GameObjectNotFound(Debug.LogWarning, "Cart", "Has not been set in the Inspector");
			}
			if (!Coffin0 || !Coffin1 || !Coffin2) {
				DebugMsg.GameObjectNotFound(Debug.LogWarning, "CoffinX", "One or more coffins have not been set in the Inspector");
			}

			foreach (LandmarkVerses v in verses) {
				if (v.LandmarkName.Contains("CART")) {
					CartLandmark.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
				} else if (v.LandmarkName.Contains("COFFIN")) {
					Coffin0.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
					Coffin1.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
					Coffin2.AddComponent<LandmarkVersesComponent>().LandmarkVerses = v;
				}
			}
		}

	}
}