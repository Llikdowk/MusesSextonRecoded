using Game;
using Game.Entities;
using Game.PlayerComponents;
using UnityEngine;

public class GlobalConfig : MonoBehaviour {

	public void Awake() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			Player.GetInstance().AddPoemVerse("A");
			Player.GetInstance().AddPoemVerse("B");
			Player.GetInstance().AddPoemVerse("C");
			Player.GetInstance().AddPoemVerse("D");
			Player.GetInstance().AddPoemVerse("E");
			Player.GetInstance().AddPoemVerse("F");
			Player.GetInstance().AddPoemVerse("G");
			Player.GetInstance().AddPoemVerse("H");
			Player.GetInstance().AddPoemVerse("I");
			GameState.CoffinsBuried = 3;
			GameObject.Find("LandmarkGIANTDOOR").GetComponent<GiantDoorComponent>().Open();
		}
	}

}