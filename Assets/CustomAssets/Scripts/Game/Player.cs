using System.Collections;
using Audio;
using Game.Entities;
using Game.PlayerComponents.Movement;
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
		[HideInInspector] public Animator Animator;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private RaycastHit _hit;
		private bool _hasHit;
		private bool _isEyeSightValid = false;
		private readonly C5.IList<string> _poem = new C5.ArrayList<string>();


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
				Animator = GetComponent<Animator>();
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
				Ray ray = new Ray(Camera.transform.position, Camera.transform.forward);
				_hasHit = Physics.SphereCast(ray, 0.05f, out _hit, 1000.0f, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
				_isEyeSightValid = true;
			}
			hit = _hit;
			return _hasHit;
		}

		public void AddPoemVerse(string verse) {
			_poem.Add(verse);
			if (_poem.Count == 9) {
				Debug.Log("OPEN GIANT DOOR");
				GameObject.Find("LandmarkGIANTDOOR").GetComponent<GiantDoorComponent>().Open();
			}
		}

		private int _currentTomb = 0;

		public string[] GetNextTombPoem() {
			if (_currentTomb < 3) {
				string[] result = _poem.View(_currentTomb * PoemState.MaxVerses, PoemState.MaxVerses).ToArray();
				++_currentTomb;
				return result;
			}
			else {
				return null;
			}
		}


		public bool PlayDigAnimation() {
			string name = "ShovelDig";
			if (Animator.GetCurrentAnimatorStateInfo(0).IsName(name)) {
				return false;
			}
			else {
				Animator.Play(name);
				return true;
			}
		}

		public void OnDigAnimationEvent() {
			AudioController.GetInstance().PlayShovel();
		}

	}
}
