using Game;
using Game.PlayerComponents;
using UnityEngine;

namespace Triggers {

	using Action = Action<PlayerAction>;

	public class TFrontCart : MonoBehaviour {

		private ActionDelegate _backupUseBehaviour;
		private Player _player;

		public void Start() {
			_player = Player.GetInstance();
		}

		private void ToggleWalkDrive() { // TOGGLE PLAYER MOVEMENT FROM FREEWALK TO DRIVECART
			PlayerState currentState = _player.CurrentState;

			if (currentState.GetType() == typeof(WalkState)) {
				_player.CurrentState = new DriveCartState();
			} 
			else if (currentState.GetType() == typeof(DriveCartState)) {
				_player.CurrentState = new WalkState();
			}
		}

		public void OnTriggerEnter(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return; 

			Debug.Log("ontriggerentered");
			Action use = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			_backupUseBehaviour = use.StartBehaviour;
			use.StartBehaviour = ToggleWalkDrive;
		}

		public void OnTriggerExit(Collider other) {
			if (other.tag != TagManager.Get(Tag.Player)) return;

			Player.GetInstance().Actions.GetAction(PlayerAction.Use).StartBehaviour = _backupUseBehaviour;

			Debug.Log("ontriggerexit");
		}
	}
}
