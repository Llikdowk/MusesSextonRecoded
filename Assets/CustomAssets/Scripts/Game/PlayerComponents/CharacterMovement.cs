using Game.CustomInput;
using UnityEngine;

namespace Game.PlayerComponents {


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		private class InputMovement {
			public Vector3 SelfDir { get; private set; }

			public void SetRawMovement() {
				ActionManager actions = Player.GetInstance().Actions;
				
				Action actionForward = actions.GetAction(ActionTag.MoveForward);
				actionForward.StartBehaviour = () => {
					SelfDir += Vector3.forward;
				};
				actionForward.FinishBehaviour = () => {
					SelfDir -= Vector3.forward;
				};
				
				Action actionBack = actions.GetAction(ActionTag.MoveBack);
				actionBack.StartBehaviour = () => {
					SelfDir += Vector3.back;
				};
				actionBack.FinishBehaviour = () => {
					SelfDir -= Vector3.back;
				};

				Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
				actionLeft.StartBehaviour = () => {
					SelfDir += Vector3.left;
				};
				actionLeft.FinishBehaviour = () => {
					SelfDir -= Vector3.left;
				};

				Action actionRight = actions.GetAction(ActionTag.MoveRight);
				actionRight.StartBehaviour = () => {
					SelfDir += Vector3.right;
				};
				actionRight.FinishBehaviour = () => {
					SelfDir -= Vector3.right;
				};
			}


			public void SetSmoothMovement() { // TODO: change FinishBehaviour by NotPressedBehaviour
				ActionManager actions = Player.GetInstance().Actions;
				
				Action actionForward = actions.GetAction(ActionTag.MoveForward);
				actionForward.StartBehaviour = () => {
					SelfDir += Vector3.forward;
				};
				actionForward.FinishBehaviour = () => {
					SelfDir -= Vector3.forward;
				};
				
				Action actionBack = actions.GetAction(ActionTag.MoveBack);
				actionBack.StartBehaviour = () => {
					SelfDir += Vector3.back;
				};
				actionBack.FinishBehaviour = () => {
					SelfDir -= Vector3.back;
				};

				Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
				actionLeft.StartBehaviour = () => {
					SelfDir += Vector3.left;
				};
				actionLeft.FinishBehaviour = () => {
					SelfDir -= Vector3.left;
				};

				Action actionRight = actions.GetAction(ActionTag.MoveRight);
				actionRight.StartBehaviour = () => {
					SelfDir += Vector3.right;
				};
				actionRight.FinishBehaviour = () => {
					SelfDir -= Vector3.right;
				};
			}

		}
		private InputMovement _input;

		public delegate void MovementInitializer();
		public float ForwardSpeed = 5.0f; // setters have to communicate with MovementBehaviour
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
		public readonly float SpeedUp = 6.0f;
		public Vector3 SelfDir { get { return _input.SelfDir.normalized; } }
		public Vector3 WorldDir { get { return transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		public Vector3 StepMovement {
			get { return _stepMovement; }
		}
		private Vector3 _stepMovement;

		public void Start() { 
			_input = new InputMovement();
			_input.SetRawMovement();
		}

		public void AddForce(Vector3 dir, float force) {
		}
		public void AddRelativeForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public void Update() {
			transform.position += _stepMovement;
			_stepMovement = Vector3.zero;
			//Vector3 speed = Vector3.zero;
			//speed.x = _selfDir.x * _selfDir.x > 0 ? RightSpeed : LeftSpeed;
			//speed.z = _selfDir.z * _selfDir.z > 0 ? ForwardSpeed : BackwardSpeed;
			_stepMovement += WorldDir * ForwardSpeed * Time.deltaTime;

		}


	}
}
