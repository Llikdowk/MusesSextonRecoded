using System.ComponentModel;
using Game.CustomInput;
using UnityEngine;

namespace Game.PlayerComponents {


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		private class InputMovement {
			public Vector3 SelfMovement { get { return _selfMovement; } }
			private Vector3 _selfMovement;

			private readonly float _speedUp, _speedDown;

			public InputMovement(float speedUp, float speedDown) {
				_speedUp = speedUp;
				_speedDown = speedDown;
			}

			public void SetRawMovement() { // TODO extract (this does not need SpeedUp/Down params!
				ActionManager actions = Player.GetInstance().Actions;
				
				Action actionForward = actions.GetAction(ActionTag.MoveForward);
				actionForward.StartBehaviour = () => {
					_selfMovement.z += 1.0f; //Vector3.forward;
				};
				actionForward.FinishBehaviour = () => {
					_selfMovement.z -= 1.0f; //Vector3.forward;
				};
				
				Action actionBack = actions.GetAction(ActionTag.MoveBack);
				actionBack.StartBehaviour = () => {
					_selfMovement.z += -1.0f; //Vector3.back;
				};
				actionBack.FinishBehaviour = () => {
					_selfMovement.z -= -1.0f; //Vector3.back;
				};

				Action actionLeft = actions.GetAction(ActionTag.MoveLeft);
				actionLeft.StartBehaviour = () => {
					_selfMovement.x += -1.0f; //Vector3.left;
				};
				actionLeft.FinishBehaviour = () => {
					_selfMovement.x -= -1.0f; //Vector3.left;
				};

				Action actionRight = actions.GetAction(ActionTag.MoveRight);
				actionRight.StartBehaviour = () => {
					_selfMovement.x += 1.0f;  //Vector3.right;
				};
				actionRight.FinishBehaviour = () => {
					_selfMovement.x -= 1.0f; //Vector3.right;
				};
			}


			public void SetSmoothMovement() { // TODO extract
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
		private InputMovement _input;

		public delegate void MovementInitializer();
		public float ForwardSpeed = 5.0f; // setters have to communicate with MovementBehaviour
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
		public float SpeedUp = 1.0f;
		public float SpeedDown = 1.0f;
		public Vector3 SelfMovement { get { return _input.SelfMovement; } }
		public Vector3 WorldMovement { get { return transform.localToWorldMatrix.MultiplyVector(_input.SelfMovement); } }
		public Vector3 SelfDir { get { return _input.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		public Vector3 StepMovement {
			get { return _stepMovement; }
		}
		private Vector3 _stepMovement;

		public void Start() { 
			_input = new InputMovement(SpeedUp, SpeedDown);
			_input.SetSmoothMovement();
		}

		public void AddForce(Vector3 dir, float force) {
		}
		public void AddRelativeForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public void Update() {
			transform.position += _stepMovement;
			_stepMovement = Vector3.zero;
			Vector3 v;
			if (WorldMovement.sqrMagnitude < 1.0f) {
				v = WorldMovement;
			}
			else {
				v = WorldDir;
			}
			_stepMovement += v * ForwardSpeed * Time.deltaTime;

		}


	}
}
