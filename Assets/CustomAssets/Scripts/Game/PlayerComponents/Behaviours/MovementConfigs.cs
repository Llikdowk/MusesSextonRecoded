using System;
using UnityEngine;

namespace Game.PlayerComponents.Behaviours {

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
		public AccelerationConfig WalkRunAcceleration;
		public CartMovementConfig DriveCartMovement;
		public AccelerationConfig CartAcceleration;
	}

}