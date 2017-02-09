using System.Collections.Generic;
using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {
	public class WalkRunMovementBehaviour : MovementBehaviour {
		private MovementConfig _currentConfig;
		private readonly Action<PlayerAction> _runAction;
		private readonly Action<PlayerAction> _useAction;
		private readonly Transform _cameraTransform;
		private readonly int _layerMaskAllButPlayer;
		private readonly List<GameObject> _outlined = new List<GameObject>();
		private readonly int _outlineLayer;
		private readonly string _coffinTag;
		private readonly string _terrainTag;
		private readonly GameObject _terrain;

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
			_terrainTag = TagManager.Get(Tag.Terrain);
			_outlineLayer = LayerMaskManager.Get(Layer.Outline);

			_runAction = Player.GetInstance().Actions.GetAction(PlayerAction.Run);
			_runAction.StartBehaviour = () => _currentConfig = runConfig;
			_runAction.FinishBehaviour = () => _currentConfig = walkConfig;
			_useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);

			_terrain = GameObject.Find("Terrain Volume");
			if (!_terrain) Debug.LogError("Terrain not found!"); // TODO extract

			/*
			_useAction.StartBehaviour = () => {
				if (_potentialUseObj == null) return;
				if (_potentialUseObj.tag == _coffinTag) {
					foreach (GameObject go in _outlined) {
						go.layer = LayerMaskManager.Get(Layer.Default);
					}
					_potentialUseObj.layer = LayerMaskManager.Get(Layer.DrawFront);
					MarkableComponent m = _potentialUseObj.GetComponent<MarkableComponent>();
					if (m != null) {
						m.DisableMark();
					}
					else {
						Debug.LogWarning("MarkComponent not found in gameObject <b>" + _potentialUseObj.gameObject.name + "</b>");
					}
					Player.GetInstance().CurrentState = new DragCoffinState(_potentialUseObj);
				}
				else if (_potentialUseObj.tag == _terrainTag) {
				}
			};
			*/
		}

		public override void Step() {
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfDir;
			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? _currentConfig.RightSpeed : _currentConfig.LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? _currentConfig.ForwardSpeed : _currentConfig.BackwardSpeed);

			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;

			CheckForInteraction();
		}

		protected virtual void CheckForInteraction() {

			Ray ray = new Ray(_transform.position, _cameraTransform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.05f, out hit, 2.5f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				GameObject g = hit.collider.gameObject;
				CleanOutline();
				if (g.tag == _coffinTag) {
					SetOutline(g);
					_useAction.StartBehaviour = () => SetDragCoffinUse(g);
				} 
				else if (g.tag == _terrainTag) {
					_useAction.StartBehaviour = SetCarveHollowUse;
				}
			}
			else {
				CleanOutline();
			}
			
		}

		private void SetDragCoffinUse(GameObject coffin) {

			CleanOutline();
			coffin.layer = LayerMaskManager.Get(Layer.DrawFront);
			MarkableComponent m = coffin.GetComponent<MarkableComponent>();
			if (m != null) {
				m.DisableMark();
			} else {
				Debug.LogWarning("MarkComponent not found in gameObject <b>" + coffin.gameObject.name + "</b>"); // TODO extract to Error class
			}
			Player.GetInstance().CurrentState = new DragCoffinState(coffin);

		}

		private void SetCarveHollowUse() {
			_terrain.GetComponent<CarveTerrainVolumeComponent>().DoCarveAction(new Ray(_transform.position, _cameraTransform.forward));
			Debug.Log("use carve action!");
		}

		private void SetOutline(GameObject g) {
			_outlined.Add(g);
			g.layer = _outlineLayer;
		}

		private void CleanOutline() {
			foreach (GameObject go in _outlined) { // TODO clean this code
				go.layer = LayerMaskManager.Get(Layer.Default);
			}
			_outlined.Clear();
		}

		public override void Clear() { // TODO: check another solution -> reset all actions in the construction before anything else?
			_runAction.Reset();
		}
	}

}