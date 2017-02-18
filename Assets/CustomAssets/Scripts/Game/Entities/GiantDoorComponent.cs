
using System.Collections;
using UnityEngine;

namespace Game.Entities {
	public class GiantDoorComponent : MonoBehaviour {

		private GameObject _leftDoor;
		private GameObject _rightDoor;

		public void Awake() {
			_leftDoor = GameObject.Find("LeftDoor").gameObject;
			_rightDoor = GameObject.Find("RightDoor").gameObject;
		}

		public void Open() {
			_leftDoor.SetActive(false);
			_rightDoor.SetActive(false);

			//StartCoroutine(DoOpen());
		}



		private IEnumerator DoOpen() {
			yield return null;
		}

	}
}
