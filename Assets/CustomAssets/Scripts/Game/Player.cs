using System.Collections;
using Audio;
using Game.Entities;
using Game.PlayerComponents.Movement;
using UnityEngine;
using Utils;


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
		private ShovelMovementComponent _shovel;


		private static Player _instance = null;

		public static Player GetInstance() {
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
				_shovel = GetComponentInChildren<ShovelMovementComponent>();
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
		}

		public string[] GetNextTombPoem() {
			if (GameState.CoffinsBuried < 3) {
				string[] result = _poem.View(GameState.CoffinsBuried * PoemState.MaxVerses, PoemState.MaxVerses).ToArray();
				return result;
			}
			return null;
		}


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

		public AnimationDelegate AnimationEnding = null;
		public void OnDigAnimationEnding() {
			if (AnimationEnding != null) {
				AnimationEnding();
				AnimationEnding = null;
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
