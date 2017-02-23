using System.Collections;
using Audio;
using Game.CameraComponents;
using Game.Entities;
using Game.PlayerComponents.Movement;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Game.PlayerComponents {

	public enum PlayerAction {
		Use, MoveForward, MoveLeft, MoveRight, MoveBack, Run
	}

	[RequireComponent(typeof(CharacterController))]
	public class Player : MonoBehaviour {

		public PlayerState CurrentState {
			get {
				return _currentState;
			}
			set {
				_currentState.OnDestroy();
				_currentState = value;
				_currentState.RunState();
			}
		}

		private PlayerState _currentState;
		public SuperConfig Config;
		public SuperLookConfig LookConfig;

		// TODO: should movement be accessible from everywhere? its changes should be limited in PlayerState classes
		// TODO: what about actions?
		[HideInInspector] public CharacterMovement CharMovement;
		[HideInInspector] public CharacterController CharController;
		[HideInInspector] public Look Look;
		[HideInInspector] public Camera MainCamera { get; private set; }
		[HideInInspector] public ActionManager<PlayerAction> Actions = new ActionManager<PlayerAction>();
		[HideInInspector] public Animator Animator;
		[HideInInspector] public CameraController CameraController;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private RaycastHit _hit;
		private bool _hasHit;
		private bool _isEyeSightValid = false;
		private ShovelMovementComponent _shovel;


		private static Player _instance = null;

		public static Player GetInstance() {
			return _instance;
		}

		public void Awake() {
			if (_instance == null) {
				_instance = this;
				DontDestroyOnLoad(gameObject);

				// TODO All of this should be GENERATED HERE instead of adquired by using GetComponent<>()
				CharMovement = GetComponent<CharacterMovement>();
				CharController = GetComponent<CharacterController>();
				MainCamera = GetComponentInChildren<Camera>();
				Animator = GetComponent<Animator>();
				_collider = GetComponent<CapsuleCollider>();
				_layerMaskAllButPlayer = ~(1 << LayerMaskManager.Get(Layer.Player));
				_shovel = GetComponentInChildren<ShovelMovementComponent>();
				Look = new Look(gameObject);
				CameraController = gameObject.AddComponent<CameraController>();

				Camera landmarkCamera = GameObject.Find("LandmarkCamera").GetComponent<Camera>();
				CameraController.Init(MainCamera, landmarkCamera);
			}
			else {
				Debug.LogWarning("Player singleton instance destroyed!");
				Destroy(gameObject);
			}
		}

		public void Start() {
			_currentState = new WalkRunState();
			_currentState.RunState();
		}

		public void Update() {
			_isEyeSightValid = false;
		}

		// Utils
		private Vector3 CalcFeetPosition(Vector3 position) {
			return position + Vector3.up * (_collider.height/2.0f + _collider.radius);
		}

		public void MoveImmediatlyTo(Vector3 position) {
			transform.position = CalcFeetPosition(position);
		}

		public void MoveSmoothlyTo(Vector3 position, float duration_s) {
			StartCoroutine(MoveSmoothlyToCoroutine(CalcFeetPosition(position), duration_s));
		}

		public IEnumerator MoveSmoothlyToCoroutine(Vector3 end, float duration_s) {
			Vector3 start = transform.position;
			float t = 0.0f;
			while (t < 1.0f) {
				transform.position = Vector3.Slerp(start, end, t);
				t += Time.deltaTime / duration_s;
				yield return null;
			}

		}

		public bool GetEyeSight(out RaycastHit hit) {
			if (!_isEyeSightValid) {
				Ray ray = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
				_hasHit = Physics.SphereCast(ray, 0.05f, out _hit, 1000.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
				_isEyeSightValid = true;
			}
			hit = _hit;
			return _hasHit;
		}

		// Poem Controller
		private readonly C5.IList<string> _poem = new C5.ArrayList<string>();
		private int _currentTombVerses = 0;
		private readonly C5.IList<string> _lastVersesChosen = new C5.ArrayList<string>();

		public void AddPoemVerse(string verse) {
			_poem.Add(verse);
		}

		public string[] GetNextTombPoem() {
			if (_currentTombVerses < 3) {
				string[] result = _poem.View(_currentTombVerses* PoemState.MaxVerses, PoemState.MaxVerses).ToArray();
				++_currentTombVerses;
				return result;
			}
			return null;
		}

		public void AddPlayerTombVerse(string verse) {
			_lastVersesChosen.Add(verse);
		}

		private int _currentVerseIndex = 0;
		public string GetPlayerTombVerse() {
			return _lastVersesChosen[_currentVerseIndex++];
		}


		// Animation Controller
		public bool PlayDigAnimation() {
			const string animationName = "ShovelDig";
			if (Animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)) {
				return false;
			}
			Animator.Play(animationName);
			return true;
		}

		public void OnDigAnimationEvent() {
			AudioController.GetInstance().PlayShovel();
		}

		public delegate void PlayerAnimation();
		public PlayerAnimation PlayerShovelAnimationEnding = null;
		public void OnDigAnimationEnding() {
			if (PlayerShovelAnimationEnding != null) {
				PlayerShovelAnimationEnding();
				PlayerShovelAnimationEnding = null;
			}
		}

		public void ShowShovel() {
			_shovel.Show();
		}

		public void HideShovel() {
			_shovel.Hide();
		}

	}
}
