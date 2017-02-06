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
	public class SmoothCfg {
		public float SpeedUp = 6.0f;
		public float SpeedDown = 6.0f;
	}

	[Serializable]
	public class SuperConfig {
		public MovementConfig WalkMovement;
		public MovementConfig RunMovement;
		public SmoothCfg SmoothAcceleration;
	}


	[RequireComponent(typeof(InputController))]
	public class CharacterMovement : MonoBehaviour {

		public SuperConfig Config;
		public MovementBehaviour MovementBehaviour;

		public bool Smooth = true;

		public Vector3 StepMovement {
			get { return MovementBehaviour.StepMovement; }
		}
		public Vector3 SelfMovement { get { return MovementBehaviour.SelfMovement; } }
		public Vector3 WorldMovement { get { return MovementBehaviour.WorldMovement; } }
		public Vector3 SelfDir { get { return MovementBehaviour.SelfDir; } }
		public Vector3 WorldDir { get { return MovementBehaviour.WorldDir; } }


		public void SetWalkBehaviour() {
			MovementBehaviour = new WalkMovementBehaviour(transform, Config);
		}
		public void SetNullBehaviour() {
			MovementBehaviour = new NullMovementBehaviour(transform);
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
