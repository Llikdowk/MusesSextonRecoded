using C5;
using Game.CameraComponents;
using Game.Entities;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement;
using Game.PlayerComponents.Movement.Behaviours;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using Game.Poems;
using UnityEngine;

namespace Game {
	public abstract class PlayerState {
		protected PlayerState() {
			_movement = Player.GetInstance().Movement;
			_transform = Player.GetInstance().transform;
			_config = Player.GetInstance().Config;
			_lookConfig = Player.GetInstance().LookConfig;
		}

		public virtual void RunState() {
		}

		protected CharacterMovement _movement;
		protected Transform _transform;
		protected SuperConfig _config;
		protected SuperLookConfig _lookConfig;
	}

	public class WalkRunState : PlayerState {
		public WalkRunState() {
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
			_movement.MovementBehaviour = new DragCoffinBehaviour(_transform, coffin, _config);
			_movement.MovementBehaviour.AddInteraction(new ThrowCoffinInteraction(coffin));

			Player.GetInstance().HideShovel();
			Player.GetInstance().Look.SetFreeLook(_lookConfig.FreeLook);
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState(GameObject cart) {
			_movement.MovementBehaviour = new CartMovementBehaviour(_transform, cart, _config);
			_movement.MovementBehaviour.AddInteraction(new StopDrivingCartInteraction());

			Player.GetInstance().HideShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.DriveScopedLook, cart.transform);
		}
	}


	public class DigState : PlayerState {
		public DigState(TombComponent tombComponent) {
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_movement.MovementBehaviour.AddInteraction(new DigInteraction(tombComponent));

			Player.GetInstance().ShowShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.DiggingScopedLook, _transform.rotation);
		}
	}

	
	public class BuryState : PlayerState {
		private TombComponent _tombComponent;
		public BuryState(TombComponent tombComponent) {
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_tombComponent = tombComponent;
		}

		public override void RunState() {
			_movement.MovementBehaviour.AddInteraction(new BuryInteraction(_tombComponent));
			Player.GetInstance().ShowShovel();
			Player.GetInstance().Look.SetScopedLook(_lookConfig.DiggingScopedLook, _transform.rotation);
			
		}
	}
	

	public class PoemState : PlayerState {
		public static IList<string> PlayerPoem;

		public enum Gender {
			Undefined, Masculine, Feminine, Plural, FirstPerson
		}

		private TombComponent _tombComponent;
		private Gender _gender;
		private int _selectedVersesCount = 0;
		public const int MaxVerses = 3;

		public PoemState(TombComponent tombComponent) {
			_gender = Gender.Undefined;
			Player.GetInstance().HideShovel();
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			SetLandmarkSelectionInteraction();
			_tombComponent = tombComponent;
		}

		public void SetGender(Gender gender) {
			_gender = gender;
		}

		public void CalcNextInteraction() {
			++_selectedVersesCount;
			if (_selectedVersesCount == MaxVerses) {
				Player.GetInstance().CurrentState = new WalkRunState();
			}
			else {
				SetLandmarkSelectionInteraction();
			}
		}

		public void SetLandmarkSelectionInteraction() {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new PoemLandmarkSelectionInteraction());
			Player.GetInstance().Camera.GetComponent<UnsaturatePostEffect>().Intensity = 1.0f;
			Player.GetInstance().Look.SetFreeLook(_lookConfig.PoemLandmarkFreeLook);
		}

		public void SetVerseInteraction(LandmarkVerses verses) {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new VerseSelectionInteraction(verses, _gender, _tombComponent));

			Player.GetInstance().Camera.GetComponent<UnsaturatePostEffect>().Intensity = 0.0f;
			Player.GetInstance().Look.SetScopedLook(_lookConfig.PoemScopedLook, _transform.rotation);
		}

	}

	public class PlayerPoemState : PlayerState {
		public PlayerPoemState() {
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_movement.MovementBehaviour.AddInteraction(new PlayerPoemInteraction());
		}
	}

	public class FinalGameState : PlayerState {
		public FinalGameState() {
			Debug.Log("GAME FINISHED");
			Player.GetInstance().CurrentState = new WalkRunState();
		}
	}


}
