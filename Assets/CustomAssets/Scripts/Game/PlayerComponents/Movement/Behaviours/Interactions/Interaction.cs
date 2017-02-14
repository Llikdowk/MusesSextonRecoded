
using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public abstract class Interaction {
		public abstract void DoInteraction();
		//public abstract void OnInteractionFinished
	}

	public class EmptyInteraction : Interaction {
		public override void DoInteraction() {
		}
	}

	public class UseCartInteraction : Interaction {
		public override void DoInteraction() {
			
		}
		
	}

	
	public class PickUpCoffinInteraction : Interaction {
		private GameObject _coffin;

		public PickUpCoffinInteraction(GameObject coffin) {
			_coffin = coffin;
		}

		public override void DoInteraction() {
			Action<PlayerAction> _useAction = Player.GetInstance().Actions.GetAction(PlayerAction.Use);
			_useAction.StartBehaviour = SetDragCoffinUse;
		}

		private void SetDragCoffinUse() {
			_coffin.layer = LayerMaskManager.Get(Layer.DrawFront);
			MarkableComponent m = _coffin.GetComponent<MarkableComponent>();
			if (m != null) {
				m.DisableMark();
			} else {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));
			}
			Player.GetInstance().CurrentState = new DragCoffinState(_coffin);

		}
	}
	/*
	public class DriveCartInteraction : Interaction {
		
	}

	*/
}
