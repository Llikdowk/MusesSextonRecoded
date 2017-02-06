using System;
using Game.PlayerComponents.Behaviours;
using UnityEngine;

namespace Game.PlayerComponents {

	[Serializable]
	public class MovementConfig {
		public float ForwardSpeed = 5.0f;
		public float BackwardSpeed = 5.0f;
		public float LeftSpeed = 5.0f;
		public float RightSpeed = 5.0f;
	}

	[Serializable]
	public class CartMovementConfig {
		public float ForwardSpeed = 7.5f;
		public float DistanceToPlayer = 1.0f;
		public float RotationMultiplier = 1.0f;

		[Range(0, 1)] public float MovementLag = 0.9f;
		[Range(0, 1)] public float LookLag = 0.9f;
	}

	[Serializable]
	public class AccelerationConfig {
		public float SpeedUp = 6.0f;
		public float SpeedDown = 6.0f;
	}

	[Serializable]
	public class SuperConfig {
		public MovementConfig WalkMovement;
		public MovementConfig RunMovement;
		public AccelerationConfig WalkAcceleration;
		public CartMovementConfig DriveCartMovement;
		public AccelerationConfig CartAcceleration;
	}


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		public SuperConfig Config;
		public MovementBehaviour MovementBehaviour;

		public Vector3 StepMovement {
			get { return MovementBehaviour.StepMovement; }
		}
		public Vector3 SelfMovement { get { return MovementBehaviour.SelfMovement; } }
		public Vector3 WorldMovement { get { return MovementBehaviour.WorldMovement; } }
		public Vector3 SelfDir { get { return MovementBehaviour.SelfDir; } }
		public Vector3 WorldDir { get { return MovementBehaviour.WorldDir; } }


		public void SetNullBehaviour() {
			Debug.Log("set NULL movBehaviour");
			MovementBehaviour.Clean();
			MovementBehaviour = new NullMovementBehaviour(transform);
		}
		public void SetWalkBehaviour() {
			Debug.Log("set WALK movBehaviour");
			MovementBehaviour.Clean();
			MovementBehaviour = new WalkMovementBehaviour(transform, Config);
		}
		public void SetCartBehaviour(GameObject cart) {
			Debug.Log("set CART movBehaviour");
			MovementBehaviour.Clean();
			MovementBehaviour = new CartMovementBehaviour(transform, cart, Config);
		}


		public void AddForce(Vector3 dir, float force) {
			MovementBehaviour.AddForce(dir, force);
		}

		public void Awake() {
			MovementBehaviour = new NullMovementBehaviour(transform);
		}

		public void Update() {
			MovementBehaviour.Step();

		}
	}


}
