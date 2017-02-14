using System.Collections.Generic;
using Audio;
using Boo.Lang.Runtime.DynamicDispatching;
using C5;
using Cubiquity;
using Game.Entities;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
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



			_useAction.StartBehaviour = () => {
				n.DoInteraction();
			};
			/*
			 *  _useAction.StartAction = () => { 
			 *  if (!CanInteract) return;
			 *  _interactions.FindMax().DoInteraction(); 
			 *  }
			 */


		}

		private Interaction n = new EmptyInteraction();
		//private C5.IntervalHeap<Interaction> _interactions = new IntervalHeap<Interaction>();
		public override void Step() {
			Vector3 SelfMovement = _transform.worldToLocalMatrix.MultiplyVector(Player.GetInstance().Controller.WorldMovementProcessed); // TODO clean this
			if (SelfMovement != Vector3.zero) { 
				AudioController.GetInstance().PlaySteps();
			}
			_transform.position += _stepMovement;
			_stepMovement = Vector3.zero;

			Vector3 dposSelf = SelfMovement.sqrMagnitude < 1.0f ? SelfMovement : SelfMovement.normalized;
			Vector3 dvelSelf = Vector3.zero;
			dvelSelf.x += dposSelf.x * (dposSelf.x > 0 ? _currentConfig.RightSpeed : _currentConfig.LeftSpeed);
			dvelSelf.z += dposSelf.z * (dposSelf.z > 0 ? _currentConfig.ForwardSpeed : _currentConfig.BackwardSpeed);

			_stepMovement += _transform.localToWorldMatrix.MultiplyVector(dvelSelf) * Time.deltaTime;

			/*
			if (CanInteract) {
				CheckForInteraction();
			}
			else {
				CleanOutline();
			}
			*/

			//_interactions.FindMax().DoInteraction();
			n.ShowFeedback();
			CheckForInteraction();

		}

		public void OnCoffinUsed(GameObject g) {
			//Interactions.add(PickUpCoffinInteraction, 100);
			Debug.Log("oncoffinused");
			n.HideFeedback();
			n = new PickUpCoffinInteraction(g);
			//CleanOutline();
		}


		/*
		public void OnCartUsed() {
			Interactions.add(DriveCartInteraction, 200);
		}
		*/
		public void OnCarveTerrain(RaycastHit hit) {
			n.HideFeedback();
			n = new CarveTerrainInteraction(hit);
		}
		

		private bool modified = false;
		protected virtual void CheckForInteraction() {
			if (!CanInteract) return;

			Ray ray = new Ray(_transform.position, _cameraTransform.forward);
			RaycastHit hit;
			//_digMarker.SetActive(false);
			if (Physics.SphereCast(ray, 0.05f, out hit, 5.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				GameObject g = hit.collider.gameObject;
				//CleanOutline();
				if (g.tag == _coffinTag && hit.distance < 2.5f) {
					modified = true;
					//SetOutline(g);
					if (n.GetType() != typeof(PickUpCoffinInteraction)) {
						OnCoffinUsed(g);
					}
					else {
						if ((n as PickUpCoffinInteraction).Coffin.GetInstanceID() != g.GetInstanceID()) {
							n.HideFeedback();
							(n as PickUpCoffinInteraction).Coffin = g;
						}
					}
				} 
				else if (g.tag == _terrainTag && hit.distance > 2.0f) {
					modified = true;
					OnCarveTerrain(hit);
				}
				else {
					n.HideFeedback();
				}
			}
			else {
				n.HideFeedback();
			}
			
		}


		/*
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
		*/

		public override void OnDestroy() { 
			//Object.Destroy(_digMarker);
		}
	}

}