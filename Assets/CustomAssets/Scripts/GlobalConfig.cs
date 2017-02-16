
using Game;
using Game.PlayerComponents;
using UnityEngine;

public class GlobalConfig : MonoBehaviour {

	public void Awake() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	//DEBUG
	public void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			Player.GetInstance().CurrentState = new PoemState();
		}
	}
}