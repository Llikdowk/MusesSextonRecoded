using UnityEngine;

namespace Game {
	class CharacterMovement : MonoBehaviour {

		private ActionManager actions = ActionManager.GetInstance();
		private Vector3 _movement;

		public float ForwardSpeed = 5.0f;
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
		public readonly float SpeedUp = 6.0f;

		public void Start() {
			Action actionForward = actions.GetAction(ActionTag.MoveForward);
			actionForward.WhileBehaviour = actionForward.DefaultWhileBehaviour = () => {
				_movement += transform.forward * Mathf.Lerp(0, ForwardSpeed, actionForward.TimeActionActive * SpeedUp) * Time.deltaTime;
			};
			
			Action actionBack = actions.GetAction(ActionTag.MoveBack);
			actionBack.WhileBehaviour = actionBack.DefaultWhileBehaviour = () => {
				_movement += -transform.forward * Mathf.Lerp(0, BackwardSpeed, actionBack.TimeActionActive * SpeedUp) * Time.deltaTime;
			};

			Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
			actionLeft.WhileBehaviour = actionLeft.DefaultWhileBehaviour = () => {
				_movement += -transform.right * Mathf.Lerp(0, LeftSpeed, actionLeft.TimeActionActive * SpeedUp) * Time.deltaTime;
			};

			Action actionRight = actions.GetAction(ActionTag.MoveRight);
			actionRight.WhileBehaviour = actionRight.DefaultWhileBehaviour = () => {
				_movement += transform.right * Mathf.Lerp(0, RightSpeed, actionRight.TimeActionActive * SpeedUp) * Time.deltaTime;
			};
		}

		public void AddForce(Vector3 dir) {
			
		}

		public void Update() {
			transform.position += _movement;
			_movement = Vector3.zero;
		}
		public void Step() {
		}

	}
}
