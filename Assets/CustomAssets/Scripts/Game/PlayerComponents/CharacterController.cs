using UnityEngine;

namespace Game.PlayerComponents {
	public enum CollisionMask {
		None = 0x000, Forward = 0x0001, Back = 0x0010, Right = 0x0100, Left = 0x1000
	}

	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class CharacterController : MonoBehaviour {

		public Vector3 Gravity = Physics.gravity;
		public float GravityMultiplier = 1.0f;
		[Range(0, 0.5f)] public float VerticalSkinWidth = 0.15f;
		[Range(0, 0.5f)] public float HorizontalSkinWidth = 0.15f;
		[Range(0, 1.0f)] public float SlopeInclinationAllowance = 0.1f;
		[Range(0, 1.0f)] public float StepAllowance = 0.0f;
		public float GrounderDistance = 2.0f;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private float _timeOnAir = 0.0f;
		private CharacterMovement _charMovement;

		private ActionManager<PlayerAction> _actions;
		private bool _isGrounded = false;

		private const int MaxSimultaneousColliders = 16;
		private const int MaxSimultaneousTriggers = 16;
		private RaycastHit[] _floorHits = new RaycastHit[MaxSimultaneousColliders];
		private RaycastHit[] _triggers = new RaycastHit[MaxSimultaneousTriggers];
		private uint _collisions;

		public void Start() {
			_actions = Player.GetInstance().Actions;
			int player = 1 << LayerMaskManager.Get(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();
		}


		private int ClassifyTriggers(ref RaycastHit[] hits, ref RaycastHit[] triggers, ref int hitsLength) {
			int triggerCount = 0;

			for (int i = hitsLength-1; i >= 0; --i) {
				if (hits[i].collider.isTrigger) {
					for (int j = i; j < hitsLength - 1; ++j) {
						hits[i] = hits[i + 1];
					}
					--hitsLength;
					++triggerCount;
				}
			}

			return triggerCount;
		}

		// minor-to-major raycast distance ordered (insertion sort, faster than default quicksort for low capacity arrays)
		private void Sort(ref RaycastHit[] hits, int start, int length) {
			RaycastHit item, nextItem, targetItem, aux;
			for (int i = start; i < length - 1; ++i) {
				item = hits[i];
				nextItem = hits[i + 1];
				if (nextItem.distance < item.distance) {
					for (int j = start; j <= i; ++j) {
						targetItem = hits[j];
						if (nextItem.distance < targetItem.distance) {
							while (j <= i) {
								aux = hits[j+1];
								hits[j+1] = hits[j];
								hits[j] = nextItem;
								nextItem = aux;
								++j;
							}
						}
					}
				} 
			}
		}

		public void Update() {
			Debug.DrawRay(transform.position, _charMovement.WorldDir, Color.cyan);
			Vector3 gravityForce = Vector3.zero;
			if (!_isGrounded) {
				gravityForce = Gravity * _timeOnAir * Time.deltaTime;
				_timeOnAir += Time.deltaTime;
			}
			Matrix4x4 M = _collider.transform.localToWorldMatrix;
			Vector3 capsuleHead = transform.position + M.MultiplyVector(_collider.center) +
			                      transform.up * (_collider.height / 2.0f - _collider.radius);
			Vector3 capsuleFeet = transform.position + M.MultiplyVector(_collider.center) -
			                      transform.up * (_collider.height / 2.0f - _collider.radius);

			
			_collisions |= CheckWalls(capsuleHead, capsuleFeet, transform.forward, PlayerAction.MoveForward) << 0; // TODO would be nice to have this independant of Action
			_collisions |= CheckWalls(capsuleHead, capsuleFeet, -transform.forward, PlayerAction.MoveBack) << 1;
			_collisions |= CheckWalls(capsuleHead, capsuleFeet, transform.right, PlayerAction.MoveRight) << 2;
			_collisions |= CheckWalls(capsuleHead, capsuleFeet, -transform.right, PlayerAction.MoveLeft) << 3;

			//CheckFloor
			int floorHitsCount = Physics.CapsuleCastNonAlloc(capsuleFeet, capsuleHead, _collider.radius, -Vector3.up, _floorHits, GrounderDistance, _layerMaskAllButPlayer);

			int triggerCount = ClassifyTriggers(ref _floorHits, ref _triggers, ref floorHitsCount);
			//Debug.Log(triggerCount + " " + floorHitsCount);

			if (floorHitsCount > 0) {
				Sort(ref _floorHits, 0, floorHitsCount);
				RaycastHit nearHit = _floorHits[0];
				if (nearHit.point == Vector3.zero) {
					transform.position = transform.position + -gravityForce + Vector3.up * VerticalSkinWidth / 2.0f;
				}
				else if (nearHit.distance > VerticalSkinWidth) {
					Debug.DrawRay(transform.position, nearHit.point - transform.position, Color.magenta);
					transform.position += Vector3.down * (nearHit.distance - VerticalSkinWidth / 2.0f);
				}
				_isGrounded = true;
				_timeOnAir = 0.0f;
			}
			else {
				_isGrounded = false;
				transform.position += gravityForce;
			}
		}


		public uint GetCollisions() {
			return _collisions;
		}

		private uint CheckWalls(Vector3 capsuleHead, Vector3 capsuleFeet, Vector3 dir, PlayerAction playerActionType) {
			int collidersFound = 0;
			Vector3 stepOffset = transform.up * StepAllowance;
			RaycastHit[] wallHits = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, dir,
				4 * HorizontalSkinWidth, _layerMaskAllButPlayer);
			foreach (RaycastHit hit in wallHits) {
				if (hit.collider.isTrigger) {
					//hit.collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
					continue;
				}
				++collidersFound;
				if (hit.point == Vector3.zero) {
					_charMovement.AddForce(hit.normal, _charMovement.StepMovement.magnitude + HorizontalSkinWidth / 2.0f);
				}
				else if (Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal)) >= (1 - SlopeInclinationAllowance)) {
					continue;
				}
				else if (hit.distance > HorizontalSkinWidth / 2.0f && hit.distance < HorizontalSkinWidth) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					_charMovement.AddForce(-dir, (1 - Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal))) * ((hit.distance - HorizontalSkinWidth / 2.0f)));
					var action = _actions.GetAction(playerActionType);
					action.Disable();
				}
			}

			if (collidersFound == 0) {
				var action = _actions.GetAction(playerActionType);
				action.Enable();
			}

			return 1;
		}

	}
}