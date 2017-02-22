using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class DragCoffinBehaviour : WalkRunMovementBehaviour { // TODO: change movement (slower)

		private readonly GameObject _coffin;
		private readonly Transform _slot;

		public DragCoffinBehaviour(Transform transform, GameObject coffin, SuperConfig config) : base(transform, config) {
			_coffin = coffin;
			_coffin.GetComponent<Rigidbody>().isKinematic = true;
			_coffin.GetComponent<Collider>().enabled = false;

			_slot = Player.GetInstance().MainCamera.transform.Find("_slotCoffin");
			if (_slot == null)
				DebugMsg.ChildObjectNotFound(Debug.LogError, "_slotCoffin");
			coffin.transform.parent = _slot;
			coffin.transform.LocalReset();
		}

	}
}
