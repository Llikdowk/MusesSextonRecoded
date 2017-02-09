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
		private readonly CarveTerrainVolumeComponent _terrainCarver;

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

			string terrainName = "Terrain Volume";
			GameObject terrain = GameObject.Find(terrainName);
			if (!terrain) DebugMsg.GameObjectNotFound(Debug.LogError, terrainName);
			_terrainCarver = terrain.GetComponent<CarveTerrainVolumeComponent>();
			if (!_terrainCarver) DebugMsg.ComponentNotFound(Debug.LogError, typeof(CarveTerrainVolumeComponent));
		}

		public override void Step() {
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfDir;
			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? _currentConfig.RightSpeed : _currentConfig.LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? _currentConfig.ForwardSpeed : _currentConfig.BackwardSpeed);

			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;

			if (CanInteract) {
				CheckForInteraction();
			}
			else {
				CleanOutline();
			}
		}

		private bool modified = false;
		protected virtual void CheckForInteraction() {
			if (!CanInteract) return;

			Ray ray = new Ray(_transform.position, _cameraTransform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.05f, out hit, 2.5f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				GameObject g = hit.collider.gameObject;
				CleanOutline();
				if (g.tag == _coffinTag) {
					modified = true;
					SetOutline(g);
					_useAction.StartBehaviour = () => SetDragCoffinUse(g);
				} 
				else if (g.tag == _terrainTag) {
					modified = true;
					_useAction.StartBehaviour = SetCarveHollowUse;
				}
				else {
					modified = false;
					_useAction.StartBehaviour = () => { };
				}
			}
			else {
				if (modified) {
					modified = false;
					_useAction.StartBehaviour = () => { };
					CleanOutline();
				}
			}
			
		}

		private void SetDragCoffinUse(GameObject coffin) {

			CleanOutline();
			coffin.layer = LayerMaskManager.Get(Layer.DrawFront);
			MarkableComponent m = coffin.GetComponent<MarkableComponent>();
			if (m != null) {
				m.DisableMark();
			} else {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));
			}
			Player.GetInstance().CurrentState = new DragCoffinState(coffin);

		}

		private void SetCarveHollowUse() {
			_terrainCarver.DoCarveAction(new Ray(_transform.position, _cameraTransform.forward));
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

		public override void ResetModifiedState() { 
			//_runAction.Reset();
		}
	}

}