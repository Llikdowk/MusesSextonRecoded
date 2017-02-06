using UnityEngine;

namespace Game.PlayerComponents {
	using Action = Action<PlayerAction>;
	
	public abstract class InputMovement {

		public Vector3 SelfMovement {
			get { return _selfMovement; }
		}
		protected Vector3 _selfMovement = Vector3.zero;

		public abstract void SetMovement();
	}


	public class RawMovement : InputMovement {

		public override void SetMovement() {
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(PlayerAction.MoveForward);
			actionForward.StartBehaviour = () => {
				_selfMovement.z += 1.0f;
			};
			actionForward.FinishBehaviour = actionForward.ForceFinishBehaviour = () => {
				_selfMovement.z = 0.0f;
			};

			Action actionBack = actions.GetAction(PlayerAction.MoveBack);
			actionBack.StartBehaviour = () => {
				_selfMovement.z += -1.0f;
			};
			actionBack.FinishBehaviour = actionBack.ForceFinishBehaviour = () => {
				_selfMovement.z = 0.0f;
			};

			Action actionLeft = actions.GetAction(PlayerAction.MoveLeft);
			actionLeft.StartBehaviour = () => {
				_selfMovement.x += -1.0f;
			};
			actionLeft.FinishBehaviour = actionLeft.ForceFinishBehaviour = () => {
				_selfMovement.x = 0.0f;
			};

			Action actionRight = actions.GetAction(PlayerAction.MoveRight);
			actionRight.StartBehaviour = () => {
				_selfMovement.x += 1.0f;
			};
			actionRight.FinishBehaviour = actionRight.ForceFinishBehaviour = () => {
				_selfMovement.x = 0.0f;
			};
		}
	}


	public class SmoothMovement : InputMovement {

		private readonly float _speedUp, _speedDown;

		public SmoothMovement(float speedUp, float speedDown) {
			_speedUp = speedUp;
			_speedDown = speedDown;
		}

		public override void SetMovement() {
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(PlayerAction.MoveForward);
			actionForward.WhileBehaviour = () => {
				_selfMovement.z = Mathf.Min(_selfMovement.z + _speedUp * Time.deltaTime, 1.0f);
			};
			actionForward.FinishBehaviour = () => {
				actionForward.NotPressedBehaviour = () => {
					_selfMovement.z = Mathf.Max(_selfMovement.z - _speedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.z == 0.0f) {
						actionForward.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionForward.ForceFinishBehaviour = () => _selfMovement.z = 0.0f;

			Action actionBack = actions.GetAction(PlayerAction.MoveBack);
			actionBack.WhileBehaviour = () => {
				_selfMovement.z = Mathf.Max(_selfMovement.z - _speedUp * Time.deltaTime, -1.0f);
			};
			actionBack.FinishBehaviour = () => {
				actionBack.NotPressedBehaviour = () => {
					_selfMovement.z = Mathf.Min(_selfMovement.z + _speedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.z == 0.0f) {
						actionBack.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionBack.ForceFinishBehaviour = () => _selfMovement.z = 0.0f;

			Action actionLeft = actions.GetAction(PlayerAction.MoveLeft);
			actionLeft.WhileBehaviour = () => {
				_selfMovement.x = Mathf.Max(_selfMovement.x - _speedUp * Time.deltaTime, -1.0f);
			};
			actionLeft.FinishBehaviour = () => {
				actionLeft.NotPressedBehaviour = () => {
					_selfMovement.x = Mathf.Min(_selfMovement.x + _speedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.x == 0.0f) {
						actionLeft.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionLeft.ForceFinishBehaviour = () => _selfMovement.x = 0.0f;

			Action actionRight = actions.GetAction(PlayerAction.MoveRight);
			actionRight.WhileBehaviour = () => {
				_selfMovement.x = Mathf.Min(_selfMovement.x + _speedUp * Time.deltaTime, 1.0f);
			};
			actionRight.FinishBehaviour = () => {
				actionRight.NotPressedBehaviour = () => {
					_selfMovement.x = Mathf.Max(_selfMovement.x - _speedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.x == 0.0f) {
						actionRight.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionRight.ForceFinishBehaviour = () => _selfMovement.x = 0.0f;
		}
	}
}
