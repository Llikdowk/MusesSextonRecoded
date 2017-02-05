using Game.CustomInput;
using UnityEngine;

namespace Game.PlayerComponents {


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

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
			//_input = new SmoothMovement(SpeedUp, SpeedDown);
			_input = new RawMovement();
			_input.SetMovement();
		}

		public void AddForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public void Update() {
			transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 v = WorldMovement.sqrMagnitude < 1.0f ? WorldMovement : WorldDir;
			_stepMovement += v * ForwardSpeed * Time.deltaTime;
		}
	}


}
