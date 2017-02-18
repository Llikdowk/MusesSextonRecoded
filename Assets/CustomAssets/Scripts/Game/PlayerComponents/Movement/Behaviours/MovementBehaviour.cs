using C5;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public abstract class MovementBehaviour {

		public Vector3 StepMovement { get { return _stepMovement; } }
		protected Vector3 _stepMovement;
		public Vector3 SelfMovement { get { return _movement.SelfMovement; } }
		public Vector3 WorldMovement { get { return _transform.localToWorldMatrix.MultiplyVector(_movement.SelfMovement); } }
		public Vector3 SelfDir { get { return _movement.SelfMovement.normalized; } }
		public Vector3 WorldDir { get { return _transform.localToWorldMatrix.MultiplyVector(SelfDir); } }

		public bool CanInteract = true;
		public Interaction CurrentInteraction = new EmptyInteraction();
		private readonly C5.ArrayList<Interaction> AvailableInteractions = new C5.ArrayList<Interaction>();

		public void AddInteraction(Interaction interaction) {
			AvailableInteractions.Add(interaction);
		}

		public void AddInteractionWithPriority(Interaction interaction) {
			AvailableInteractions.Insert(0, interaction);
		}

		public void RemoveInteraction(Interaction interaction) {
			try {
				AvailableInteractions.Remove(interaction);
			}
			catch (NoSuchItemException) {
				DebugMsg.NoExistantInteraction(Debug.LogWarning);
			}
		}

		public void ClearInteractions() {
			AvailableInteractions.Clear();
		}

		protected MovementHandler _movement = new NullMovementHandler();
		protected Transform _transform;

		protected MovementBehaviour(Transform transform) {
			Player.GetInstance().Actions.ResetAllActions();
			_transform = transform;

			var useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			useAction.StartBehaviour = () => {
				CurrentInteraction.DoInteraction();
			};
		}

		public virtual void Step() {
			if (CanInteract) {
				InteractionStep();
			}
		}

		protected void InteractionStep() {
			Interaction potentialInteraction = null;

			foreach (Interaction interaction in AvailableInteractions) {
				potentialInteraction = interaction.CheckForPromotion();
				if (potentialInteraction != null) {
					CurrentInteraction.HideFeedback();
					CurrentInteraction = potentialInteraction;
					CurrentInteraction.ShowFeedback();
					break;
				}
			}
			if (potentialInteraction == null) {
				CurrentInteraction.HideFeedback();
				CurrentInteraction = new EmptyInteraction();
			}
		}

		public abstract void OnDestroy();
	}

}