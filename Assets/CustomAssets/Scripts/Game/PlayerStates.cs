using Audio;
using C5;
using Game.Entities;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement;
using Game.PlayerComponents.Movement.Behaviours;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using Game.Poems;
using UnityEngine;

namespace Game {
	public abstract class PlayerState { // TODO: all states could be sotred here as static vars
		protected PlayerState() { 
			_movement = Player.GetInstance().CharMovement;
			_transform = Player.GetInstance().transform;
			_config = Player.GetInstance().Config;
			_lookConfig = Player.GetInstance().LookConfig;
		}

		public abstract void RunState();

		public virtual void OnDestroy() {
		}

		protected CharacterMovement _movement;
		protected Transform _transform;
		protected SuperConfig _config;
		protected SuperLookConfig _lookConfig;
	}

	public class NullState : PlayerState {
		public static readonly NullState State = new NullState();

		private NullState() {
		}

		public override void RunState() {
		}
	}

	public class WalkRunState : PlayerState {
		public WalkRunState() {
		}

		public override void RunState() {
			_movement.MovementBehaviour = new WalkRunMovementBehaviour(_transform, _config);
			_movement.MovementBehaviour.AddInteraction(new PickUpCoffinInteraction());
			_movement.MovementBehaviour.AddInteraction(new CarveTerrainInteraction());
			Player.GetInstance().ShowShovel();
			Player.GetInstance().Look.SetFreeLook(_lookConfig.FreeLook);
		}
	}

	public class DragCoffinState : PlayerState {
		public GameObject Coffin { get; private set; }
		public DragCoffinState(GameObject coffin) {
			Coffin = coffin;
		}

		public override void RunState() {
			_movement.MovementBehaviour = new DragCoffinBehaviour(_transform, Coffin, _config);
			_movement.MovementBehaviour.AddInteraction(new ThrowCoffinInteraction(Coffin));
			Player.GetInstance().HideShovel();
			Player.GetInstance().Look.SetFreeLook(_lookConfig.FreeLook);
		}
	}

	public class DriveCartState : PlayerState {
		private GameObject _cart;

		public DriveCartState(GameObject cart) {
			_cart = cart;
		}

		public override void RunState() {
			_movement.MovementBehaviour = new CartMovementBehaviour(_transform, _cart, _config);
			_movement.MovementBehaviour.AddInteraction(new StopDrivingCartInteraction());

			Player.GetInstance().HideShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.DriveScopedLook, _cart.transform);
		}
	}


	public class DigState : PlayerState {
		private TombComponent _tombComponent;
		public DigState(TombComponent tombComponent) {
			_tombComponent = tombComponent;
		}

		public override void RunState() {
			_movement.MovementBehaviour = new NullMovementBehaviour();
			_movement.MovementBehaviour.AddInteraction(new DigInteraction(_tombComponent));
			Player.GetInstance().ShowShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.DiggingScopedLook, _transform.rotation);
		}
	}

	public class PoemState : PlayerState {
		public static IList<string> PlayerPoem;

		public enum GenderEnum {
			Undefined, Masculine, Feminine, Plural, FirstPerson
		}

		private readonly TombComponent _tombComponent;
		public GenderEnum Gender;
		public const int MaxVerses = 3;

		public PoemState(TombComponent tombComponent) {
			Gender = GenderEnum.Undefined;
			_tombComponent = tombComponent;
			AudioController.GetInstance().FadeInPercussion();
			IconMarkerComponent.HideAll();
		}

		public override void OnDestroy() {
			AudioController.GetInstance().FadeOutPercussion();
			IconMarkerComponent.ShowAll();
			Player.GetInstance().CameraController.Unsaturate(0.0f, 0.5f);
			Player.GetInstance().CameraController.DisableDepthOfField(0.5f);
		}

		public override void RunState() {
			Player.GetInstance().HideShovel();
			_movement.MovementBehaviour = new NullMovementBehaviour();
			SetLandmarkSelectionInteraction();
		}

		public void SetLandmarkSelectionInteraction() {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new PoemLandmarkSelectionInteraction());
			Player.GetInstance().CameraController.Unsaturate(1.0f, 1.0f);
			Player.GetInstance().CameraController.DisableDepthOfField(0.5f);
			Player.GetInstance().Look.SetFreeLook(_lookConfig.PoemLandmarkFreeLook);
		}

		public void SetVerseInteraction(LandmarkVerses verses) {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new VerseSelectionInteraction(verses, Gender, _tombComponent));

			Player.GetInstance().CameraController.EnableDepthOfField(0.5f);
			Player.GetInstance().Look.SetScopedLook(_lookConfig.PoemScopedLook, Player.GetInstance().MainCamera.transform.rotation);
		}
	}

	public class PlayerPoemState : PlayerState {
		public PlayerPoemState() {
			IconMarkerComponent.HideAll();
		}

		public override void OnDestroy() {
			AudioController.GetInstance().FadeOutMusic3(5.0f);
			Player.GetInstance().CameraController.DisableDepthOfField(0.5f);
		}

		public override void RunState() {
			Player.GetInstance().HideShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.PoemScopedLook, Player.GetInstance().MainCamera.transform.rotation);
			Player.GetInstance().CameraController.EnableDepthOfField(0.5f);
			_movement.MovementBehaviour = new NullMovementBehaviour();
			_movement.MovementBehaviour.AddInteraction(new PlayerPoemInteraction());
		}
	}
}