using Audio;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {
	public class WalkRunMovementBehaviour : MovementBehaviour {
		private MovementConfig _currentConfig;
		private readonly Transform _cameraTransform;
		private readonly int _layerMaskAllButPlayer;
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

			var runAction = Player.GetInstance().Actions.GetAction(PlayerAction.Run);
			runAction.StartBehaviour = () => _currentConfig = runConfig;
			runAction.FinishBehaviour = () => _currentConfig = walkConfig;
			var useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);

			useAction.StartBehaviour = () => {
				Interaction.DoInteraction();
			};

		}

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

			Interaction.ShowFeedback();
			CheckForInteraction();

		}
		

		protected virtual void CheckForInteraction() {
			if (!CanInteract) return;

			Ray ray = new Ray(_transform.position, _cameraTransform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.05f, out hit, 5.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) {
				GameObject g = hit.collider.gameObject;
				if (g.tag == _coffinTag && hit.distance < 2.5f) {
					if (Interaction.GetType() != typeof(PickUpCoffinInteraction)) {
						Interaction = new PickUpCoffinInteraction(g);
					}
					else {
						if ((Interaction as PickUpCoffinInteraction).Coffin.GetInstanceID() != g.GetInstanceID()) {
							Interaction.HideFeedback();
							(Interaction as PickUpCoffinInteraction).Coffin = g;
						}
					}
				} 
				else if (g.tag == _terrainTag && hit.distance > 2.0f) {
					if (Interaction.GetType() != typeof(CarveTerrainInteraction)) {
						Interaction = new CarveTerrainInteraction(hit);
					}
					else {
						(Interaction as CarveTerrainInteraction).RefreshHit(hit);
					}
				}
				else {
					Interaction.HideFeedback();
				}
			}
			else {
				Interaction.HideFeedback();
			}
			
		}

		public override void OnDestroy() { 
		}
	}

}