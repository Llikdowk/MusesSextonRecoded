using UnityEngine;

namespace Game.PlayerComponents {
	public abstract class InputMovement {
		public Vector3 SelfMovement {
			get { return _selfMovement; }
		}
		protected Vector3 _selfMovement = Vector3.zero;

		public abstract void SetMovement();
	}


	public class RawMovement : InputMovement {

		public override void SetMovement() {
			ActionManager actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(ActionTag.MoveForward);
			actionForward.StartBehaviour = () => {
				_selfMovement.z += 1.0f;
			};
			actionForward.FinishBehaviour = () => {
				_selfMovement.z -= 1.0f;
			};

			Action actionBack = actions.GetAction(ActionTag.MoveBack);
			actionBack.StartBehaviour = () => {
				_selfMovement.z += -1.0f;
			};
			actionBack.FinishBehaviour = () => {
				_selfMovement.z -= -1.0f;
			};

			Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
			actionLeft.StartBehaviour = () => {
				_selfMovement.x += -1.0f;
			};
			actionLeft.FinishBehaviour = () => {
				_selfMovement.x -= -1.0f;
			};

			Action actionRight = actions.GetAction(ActionTag.MoveRight);
			actionRight.StartBehaviour = () => {
				_selfMovement.x += 1.0f;
			};
			actionRight.FinishBehaviour = () => {
				_selfMovement.x -= 1.0f;
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
			ActionManager actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(ActionTag.MoveForward);
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

			Action actionBack = actions.GetAction(ActionTag.MoveBack);
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

			Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
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

			Action actionRight = actions.GetAction(ActionTag.MoveRight);
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
		}
	}
}
