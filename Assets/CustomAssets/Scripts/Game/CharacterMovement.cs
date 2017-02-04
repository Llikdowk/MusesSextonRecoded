﻿using Game.CustomInput;
using UnityEngine;

namespace Game {
	[RequireComponent(typeof(InputController))]
	class CharacterMovement : MonoBehaviour {

		public float ForwardSpeed = 5.0f;
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
		public readonly float SpeedUp = 6.0f;

		public Vector3 MovementDirection {
			get { return _stepMovement.normalized; }
		}

		public Vector3 MovementLocalDirection {
			get { return transform.worldToLocalMatrix.MultiplyVector(_stepMovement).normalized; }
		}

		public Vector3 StepMovement {
			get { return _stepMovement; }
		}
		private Vector3 _stepMovement;

		private readonly ActionManager _actions = ActionManager.GetInstance();


		public void Start() {

			Action actionForward = _actions.GetAction(ActionTag.MoveForward);
			actionForward.WhileBehaviour = actionForward.DefaultWhileBehaviour = () => {
				_stepMovement += transform.forward * Mathf.Lerp(0, ForwardSpeed, actionForward.TimeActionActive * SpeedUp) * Time.deltaTime;
			};
			
			Action actionBack = _actions.GetAction(ActionTag.MoveBack);
			actionBack.WhileBehaviour = actionBack.DefaultWhileBehaviour = () => {
				_stepMovement += -transform.forward * Mathf.Lerp(0, BackwardSpeed, actionBack.TimeActionActive * SpeedUp) * Time.deltaTime;
			};

			Action actionLeft = _actions.GetAction(ActionTag.MoveLeft);
			actionLeft.WhileBehaviour = actionLeft.DefaultWhileBehaviour = () => {
				_stepMovement += -transform.right * Mathf.Lerp(0, LeftSpeed, actionLeft.TimeActionActive * SpeedUp) * Time.deltaTime;
			};

			Action actionRight = _actions.GetAction(ActionTag.MoveRight);
			actionRight.WhileBehaviour = actionRight.DefaultWhileBehaviour = () => {
				_stepMovement += transform.right * Mathf.Lerp(0, RightSpeed, actionRight.TimeActionActive * SpeedUp) * Time.deltaTime;
			};
		}

		public void AddForce(Vector3 dir, float force) {
				_stepMovement += dir * force;
		}

		public void Update() {
			transform.position += _stepMovement;
			_stepMovement = Vector3.zero;
		}

	}
}
