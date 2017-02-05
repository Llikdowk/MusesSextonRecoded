using UnityEngine;

namespace Game.PlayerComponents {
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {
		private static Player _instance;
		public static Player GetInstance() {return _instance; }

		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;

		public void Awake() {
			if (_instance != null && _instance != this) {
				Destroy(this.gameObject);
			}
			else {
				_instance = this;
				Movement = GetComponent<CharacterMovement>();
				Controller = GetComponent<CharacterController>();
			}
		}
	}
}
