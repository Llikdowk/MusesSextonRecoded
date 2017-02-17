
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
		}

		protected CharacterMovement _movement;
		protected Transform _transform;
		protected SuperConfig _config;
	}

	public class WalkRunState : PlayerState {
		public WalkRunState() {
			_movement.MovementBehaviour = new WalkRunMovementBehaviour(_transform, _config);
			_movement.MovementBehaviour.AddInteraction(new PickUpCoffinInteraction());
			_movement.MovementBehaviour.AddInteraction(new CarveTerrainInteraction());
		}
	}

	public class DragCoffinState : PlayerState {
		public GameObject Coffin { get; private set; }
		public DragCoffinState(GameObject coffin) {
			Coffin = coffin;
			_movement.MovementBehaviour = new DragCoffinBehaviour(_transform, coffin, _config);
			_movement.MovementBehaviour.AddInteraction(new ThrowCoffinInteraction(coffin));
		}
	}

	public class DriveCartState : PlayerState {
		public DriveCartState(GameObject cart) {
			_movement.MovementBehaviour = new CartMovementBehaviour(_transform, cart, _config);
			_movement.MovementBehaviour.AddInteraction(new StopDrivingCartInteraction());
		}
	}


	public class DigState : PlayerState {
		public DigState(GameObject ground) {
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_movement.MovementBehaviour.AddInteraction(new DigInteraction(ground));
		}
	}

	public class BuryState : PlayerState {
		public BuryState(GameObject tomb, GameObject ground) {
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_movement.MovementBehaviour.AddInteraction(new BuryInteraction(tomb, ground));
		}
	}

	public class PoemState : PlayerState {
		public enum Gender {
			Undefined, Masculine, Feminine, Plural, FirstPerson
		}

		private Gender _gender;

		public PoemState() {
			_gender = Gender.Undefined;
			_movement.MovementBehaviour = new NullMovementBehaviour(_transform);
			_movement.MovementBehaviour.AddInteraction(new PoemLandmarkSelectionInteraction());
		}

		public void SetGender(Gender gender) {
			_gender = gender;
		}

		public void SetLandmarkSelectionInteraction() {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new PoemLandmarkSelectionInteraction());
		}

		public void SetVerseInteraction(LandmarkVerses verses) {
			_movement.MovementBehaviour.ClearInteractions();
			_movement.MovementBehaviour.AddInteraction(new VerseSelectionInteraction(verses, _gender));
		}

	}


}
