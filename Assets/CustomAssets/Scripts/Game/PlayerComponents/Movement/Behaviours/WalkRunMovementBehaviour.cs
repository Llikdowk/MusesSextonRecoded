using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {
	public class WalkRunMovementBehaviour : MovementBehaviour {
		private MovementConfig _currentConfig;
		private readonly Action<PlayerAction> _runAction;
		private readonly Action<PlayerAction> _useAction;
		private readonly Transform _cameraTransform;
		private readonly int _layerMaskAllButPlayer;
		private readonly List<GameObject> outlined = new List<GameObject>();
		private readonly int _outlineLayer;
		private readonly string _coffinTag;
		private GameObject _potentialUseObj = null;

		public WalkRunMovementBehaviour(Transform transform, SuperConfig config) : base(transform) {
			Player.GetInstance().Look.Config = config.WalkRunLook;
			MovementConfig walkConfig = config.WalkMovement;
			MovementConfig runConfig = config.RunMovement;
			_movement = new SmoothMovementHandler(config.WalkRunAcceleration);
			_movement.SetMovement();
			_currentConfig = walkConfig;
			_cameraTransform = Player.GetInstance().Camera.transform;
			_layerMaskAllButPlayer = ~(1 << LayerMaskManager.Get(Layer.Player));
			_coffinTag = TagManager.Get(Tag.Coffin);
			_outlineLayer = LayerMaskManager.Get(Layer.Outline);

			_runAction = Player.GetInstance().Actions.GetAction(PlayerAction.Run);
			_runAction.StartBehaviour = () => _currentConfig = runConfig;
			_runAction.FinishBehaviour = () => _currentConfig = walkConfig;
			_useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);

			_useAction.StartBehaviour = () => {
				if (_potentialUseObj == null) return;
				if (_potentialUseObj.tag == _coffinTag) {
					foreach (GameObject go in outlined) {
						go.layer = LayerMaskManager.Get(Layer.Default);
					}
					_potentialUseObj.layer = LayerMaskManager.Get(Layer.DrawFront);
					Player.GetInstance().CurrentState = new DragCoffinState(_potentialUseObj);
				}
			};
		}

		public override void Step() {
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfDir;
			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? _currentConfig.RightSpeed : _currentConfig.LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? _currentConfig.ForwardSpeed : _currentConfig.BackwardSpeed);

			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;

			Calcs();
		}

		protected virtual void Calcs() {

			Ray ray = new Ray(_transform.position, _cameraTransform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.05f, out hit, 2.5f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				GameObject g = hit.collider.gameObject;
				foreach (GameObject go in outlined) { // TODO clean this code
					go.layer = LayerMaskManager.Get(Layer.Default);
				}
				outlined.Clear();
				if (g.tag == _coffinTag) {
					_potentialUseObj = g;
					g.layer = _outlineLayer;
					outlined.Add(g);
				}
			}
			else {
				foreach (GameObject go in outlined) {
					go.layer = LayerMaskManager.Get(Layer.Default);
				}
				_potentialUseObj = null;
				outlined.Clear();
			}
			
		}

		public override void Clear() {
			_runAction.Reset();
		}
	}

}