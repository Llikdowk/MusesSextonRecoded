using Game.PlayerComponents.Movement;
using Game.PlayerComponents.Movement.Behaviours;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using UnityEngine;


namespace Game.PlayerComponents {

	public enum PlayerAction {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack, Run
	}

	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {

		public PlayerState CurrentState;
		public SuperConfig Config;

		// TODO: should movement be accessible from everywhere? its changes should be limited in PlayerState classes
		// TODO: what about actions?
		[HideInInspector] public CharacterMovement Movement;
		[HideInInspector] public CharacterController Controller;
		[HideInInspector] public Look Look;
		[HideInInspector] public Camera Camera { get; private set; }
		[HideInInspector] public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();
		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private RaycastHit _hit;
		private bool _hasHit;
		private bool _isEyeSightValid = false;


		private static Player _instance = null;

		public static Player GetInstance() {
			if (_instance == null) {
				Debug.LogError("Player singleton called but it is not yet built"); // TODO extract to DebugMsg
			}
			return _instance;
		}

		public void Awake() {
			if (_instance == null) {
				_instance = this;
				DontDestroyOnLoad(gameObject);

				Movement = GetComponent<CharacterMovement>();
				Controller = GetComponent<CharacterController>();
				Look = GetComponent<Look>();
				Camera = GetComponentInChildren<Camera>();
				_collider = GetComponent<CapsuleCollider>();
				_layerMaskAllButPlayer = ~(1 << LayerMaskManager.Get(Layer.Player));
			}
			else {
				Debug.LogWarning("Player singleton instance destroyed!");
				Destroy(gameObject);
			}
		}

		public void Start() {
			CurrentState = new WalkRunState();
		}

		public void Update() {
			_isEyeSightValid = false;
		}

		public void MoveImmediatlyTo(Vector3 position) {
			transform.position = position + Vector3.up * _collider.height/2.0f;
		}

		public bool GetEyeSight(out RaycastHit hit) {
			if (!_isEyeSightValid) {
				Ray ray = new Ray(transform.position, Camera.transform.forward);
				_hasHit = Physics.SphereCast(ray, 0.05f, out _hit, 5.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
				_isEyeSightValid = true;
			}
			hit = _hit;
			return _hasHit;
		}
	}
}
