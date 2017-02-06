using UnityEngine;

namespace Game.PlayerComponents {


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		private MovementHandler _movement;

		public bool Smooth = true;
		public float ForwardSpeed = 5.0f; // setters have to communicate with MovementBehaviour
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
		public float SpeedUp = 1.0f;
		public float SpeedDown = 1.0f;
		public float RunMultiplier = 1.5f;
		public Vector3 SelfMovement { get { return _movement.SelfMovement; } }
		public Vector3 WorldMovement { get { return transform.localToWorldMatrix.MultiplyVector(_movement.SelfMovement); } }
		public Vector3 SelfDir { get { return _movement.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		public Vector3 StepMovement {
			get { return _stepMovement; }
		}
		private Vector3 _stepMovement;

		public void Start() {
			if (Smooth) {
				_movement = new SmoothMovementHandler(SpeedUp, SpeedDown);
			}
			else {
				_movement = new RawMovementHandler();
			}
			_movement.SetMovement();
			var run =  Player.GetInstance().Actions.GetAction(PlayerAction.Run);
			run.StartBehaviour = () => {
				ForwardSpeed *= RunMultiplier;
				BackwardSpeed *= RunMultiplier;
				LeftSpeed *= RunMultiplier;
				RightSpeed *= RunMultiplier;
			};
			run.FinishBehaviour = run.ForceFinishBehaviour = () => {
				ForwardSpeed /= RunMultiplier;
				BackwardSpeed /= RunMultiplier;
				LeftSpeed /= RunMultiplier;
				RightSpeed /= RunMultiplier;
			};
		}

		public void AddForce(Vector3 dir, float force) {
			_stepMovement += dir * force;
		}

		public void Update() {
			transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfDir;
			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? RightSpeed : LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? ForwardSpeed : BackwardSpeed);
			_stepMovement += transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;
		}
	}


}
