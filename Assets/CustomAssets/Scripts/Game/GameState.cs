using Audio;
using Game.Entities;
using Game.PlayerComponents;
using UnityEngine;
using Utils;

namespace Game {
	public class GameState {
		public static int CoffinsBuried {
			get { return _coffinsBuried; }
			set {
				_coffinsBuried = value;
				if (_coffinsBuried == 3) {
					AudioController.GetInstance().FadeOutMusic2(5.0f, () => {
						AudioController.GetInstance().FadeOutMusic1(5.0f);
					});
					GiantDoorComponent door = GameObject.Find("LandmarkGIANTDOOR").GetComponent<GiantDoorComponent>();
					door.Open();
					Player player = Player.GetInstance();
					AnimationUtils.LookTowardsHorizontal(player.transform,
						(door.transform.position - player.transform.position).normalized, 1.0f);
					AnimationUtils.LookTowardsVertical(player.MainCamera.transform, door.transform.position, 1.0f);
				}
			}
		}

		private static int _coffinsBuried = 0;
	}
}
