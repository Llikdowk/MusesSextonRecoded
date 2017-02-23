using Game.Entities;
using UnityEngine;

namespace Game {
	public class GameState {
		public static int CoffinsBuried {
			get { return _coffinsBuried; }
			set {
				_coffinsBuried = value;
				if (_coffinsBuried == 3) {
					GameObject.Find("LandmarkGIANTDOOR").GetComponent<GiantDoorComponent>().Open();
				}
			}
		}

		private static int _coffinsBuried = 0;
	}
}
