using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours {

	public class DragCoffinBehaviour : WalkRunMovementBehaviour { // TODO: change movement (slower)

		private readonly GameObject _coffin;
		private readonly Transform _slot;

		public DragCoffinBehaviour(Transform transform, GameObject coffin, SuperConfig config) : base(transform, config) {
			_coffin = coffin;
			_coffin.GetComponent<Rigidbody>().isKinematic = true;
			_coffin.GetComponent<Collider>().enabled = false;

			_slot = Player.GetInstance().transform.Find("Camera/_slotCoffin");
			if (_slot == null) Debug.LogError("Camera/_slotCoffin not found in PlayerGameobject"); // TODO extract to Error class
			coffin.transform.parent = _slot;
			coffin.transform.LocalReset();
			Player.GetInstance().HideShovel();
		}

	}
}
