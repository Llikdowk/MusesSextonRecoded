﻿using UnityEngine;

namespace Game.PlayerComponents.Movement {
	using Action = Action<PlayerAction>;
	
	public abstract class MovementHandler {

		public Vector3 SelfMovement {
			get { return _selfMovement; }
		}
		protected Vector3 _selfMovement = Vector3.zero;
		public abstract MovementHandler SetMovement();
	}

	public class NullMovementHandler : MovementHandler {
		public override MovementHandler SetMovement() {
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;
			actions.GetAction(PlayerAction.MoveForward).Reset();
			actions.GetAction(PlayerAction.MoveBack).Reset();
			actions.GetAction(PlayerAction.MoveLeft).Reset();
			actions.GetAction(PlayerAction.MoveRight).Reset();
			return this;
		}
	}

	public class RawMovementHandler : MovementHandler {

		public override MovementHandler SetMovement() {
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(PlayerAction.MoveForward).Reset();
			actionForward.StartBehaviour = () => {
				_selfMovement.z += 1.0f;
			};
			actionForward.FinishBehaviour = actionForward.ForceFinishBehaviour = () => {
				_selfMovement.z = 0.0f;
			};

			Action actionBack = actions.GetAction(PlayerAction.MoveBack).Reset();
			actionBack.StartBehaviour = () => {
				_selfMovement.z += -1.0f;
			};
			actionBack.FinishBehaviour = actionBack.ForceFinishBehaviour = () => {
				_selfMovement.z = 0.0f;
			};

			Action actionLeft = actions.GetAction(PlayerAction.MoveLeft).Reset();
			actionLeft.StartBehaviour = () => {
				_selfMovement.x += -1.0f;
			};
			actionLeft.FinishBehaviour = actionLeft.ForceFinishBehaviour = () => {
				_selfMovement.x = 0.0f;
			};

			Action actionRight = actions.GetAction(PlayerAction.MoveRight).Reset();
			actionRight.StartBehaviour = () => {
				_selfMovement.x += 1.0f;
			};
			actionRight.FinishBehaviour = actionRight.ForceFinishBehaviour = () => {
				_selfMovement.x = 0.0f;
			};

			return this;
		}
	}


	public class SmoothMovementHandler : MovementHandler {

		private readonly AccelerationConfig _config;

		public SmoothMovementHandler(AccelerationConfig config) {
			this._config = config;
		}

		public override MovementHandler SetMovement() {
			ActionManager<PlayerAction> actions = Player.GetInstance().Actions;

			Action actionForward = actions.GetAction(PlayerAction.MoveForward).Reset();
			actionForward.WhileBehaviour = () => {
				_selfMovement.z = Mathf.Min(_selfMovement.z + _config.SpeedUp * Time.deltaTime, 1.0f);
			};
			actionForward.FinishBehaviour = () => {
				actionForward.NotPressedBehaviour = () => {
					_selfMovement.z = Mathf.Max(_selfMovement.z - _config.SpeedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.z == 0.0f) {
						actionForward.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionForward.ForceFinishBehaviour = () => {
				if (_selfMovement.z > 0) _selfMovement.z = 0.0f;
			};

			Action actionBack = actions.GetAction(PlayerAction.MoveBack).Reset();
			actionBack.WhileBehaviour = () => {
				_selfMovement.z = Mathf.Max(_selfMovement.z - _config.SpeedUp * Time.deltaTime, -1.0f);
			};
			actionBack.FinishBehaviour = () => {
				actionBack.NotPressedBehaviour = () => {
					_selfMovement.z = Mathf.Min(_selfMovement.z + _config.SpeedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.z == 0.0f) {
						actionBack.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionBack.ForceFinishBehaviour = () => {
				if (_selfMovement.z < 0) _selfMovement.z = 0.0f;
			};

			Action actionLeft = actions.GetAction(PlayerAction.MoveLeft).Reset();
			actionLeft.WhileBehaviour = () => {
				_selfMovement.x = Mathf.Max(_selfMovement.x - _config.SpeedUp * Time.deltaTime, -1.0f);
			};
			actionLeft.FinishBehaviour = () => {
				actionLeft.NotPressedBehaviour = () => {
					_selfMovement.x = Mathf.Min(_selfMovement.x + _config.SpeedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.x == 0.0f) {
						actionLeft.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionLeft.ForceFinishBehaviour = () => {
				if (_selfMovement.x < 0) _selfMovement.x = 0.0f;
			};

			Action actionRight = actions.GetAction(PlayerAction.MoveRight).Reset();
			actionRight.WhileBehaviour = () => {
				_selfMovement.x = Mathf.Min(_selfMovement.x + _config.SpeedUp * Time.deltaTime, 1.0f);
			};
			actionRight.FinishBehaviour = () => {
				actionRight.NotPressedBehaviour = () => {
					_selfMovement.x = Mathf.Max(_selfMovement.x - _config.SpeedDown * Time.deltaTime, 0.0f);
					if (_selfMovement.x == 0.0f) {
						actionRight.NotPressedBehaviour = Action.nop;
					}
				};
			};
			actionRight.ForceFinishBehaviour = () => {
				if (_selfMovement.x > 0) _selfMovement.x = 0.0f;
			};

			return this;
		}
	}
}
