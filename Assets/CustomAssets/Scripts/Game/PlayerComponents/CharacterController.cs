using System.Runtime.CompilerServices;
using UnityEditor;
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
		private RaycastHit[] _colliderHits = new RaycastHit[MaxSimultaneousColliders];
		private RaycastHit[] _triggers = new RaycastHit[MaxSimultaneousTriggers];
		private uint _collisions;

		public void Start() {
			_actions = Player.GetInstance().Actions;
			int player = 1 << LayerMaskManager.Get(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();

		}

		private int DeleteTriggers(ref RaycastHit[] hits, ref int hitsLength) {
			int triggerCount = 0;

			for (int i = hitsLength-1; i >= 0; --i) {
				if (hits[i].collider.isTrigger) {
					for (int j = i; j < hitsLength - 1; ++j) {
						hits[i] = hits[i + 1];
					}
					//--hitsLength;
					++triggerCount;
				}
			}

			hitsLength = hitsLength - triggerCount;
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
							while (j <= i + 1) {
								aux = hits[j];
								hits[j] = nextItem;
								nextItem = aux;
								++j;
							}
						}
					}
				}
			}
		}

		//method from https://forum.unity3d.com/threads/spherecast-capsulecast-raycasthit-normal-is-not-the-surface-normal-as-the-documentation-states.275369/
        private static void RepairHitSurfaceNormal(ref RaycastHit hit)
        {
            if(hit.collider is MeshCollider)
            {
                var collider = hit.collider as MeshCollider;
                var mesh = collider.sharedMesh;
                var tris = mesh.triangles;
                var verts = mesh.vertices;
 
                var v0 = verts[tris[hit.triangleIndex * 3]];
                var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
                var v2 = verts[tris[hit.triangleIndex * 3 + 2]];
 
                var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;
                hit.normal = hit.transform.TransformDirection(n);
            }
            else
            {
                var p = hit.point + hit.normal * 0.01f;
	            hit.collider.Raycast(new Ray(p, -hit.normal), out hit, 0.011f);
                //collider.Raycast(p, -hit.normal, out hit, 0.011f, layerMask);
            }
        }

		private Vector3 _direction;
		private Vector3 _lastPosition;
		public float tolerance0 = -0.9f;
		public float tolerance = -0.4f;
		public Vector3 WorldMovementProcessed = Vector3.zero;
		public void Update() {
			float _speed = (transform.position - _lastPosition).magnitude;
			_direction = (transform.position - _lastPosition).normalized;
			_lastPosition = transform.position;
			Debug.DrawRay(transform.position, _charMovement.WorldDir, Color.cyan);
			Vector3 gravityForce = Vector3.zero;
			if (!_isGrounded) {
				gravityForce = Gravity * _timeOnAir * Time.deltaTime; // TODO: move to characterMovement?
				_timeOnAir += Time.deltaTime;
			}
			Matrix4x4 M = _collider.transform.localToWorldMatrix; //TODO: use transform.transformpoint instead
			Vector3 capsuleHead = transform.position + M.MultiplyVector(_collider.center) +
			                      transform.up * (_collider.height / 2.0f - _collider.radius); 
			Vector3 capsuleFeet = transform.position + M.MultiplyVector(_collider.center) -
			                      transform.up * (_collider.height / 2.0f - _collider.radius);

			
			// Vertical Collision
			int hitsCount = Physics.CapsuleCastNonAlloc(capsuleFeet, capsuleHead, _collider.radius, Vector3.down,
				_colliderHits, GrounderDistance, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);

			if (hitsCount > 0) {
				Sort(ref _colliderHits, 0, hitsCount);
				RaycastHit nearHit = _colliderHits[0];
				if (nearHit.point == Vector3.zero) {
					const float pushBack = 10.0f;
					if (Vector3.Dot(nearHit.normal, Vector3.up) >= 1-SlopeInclinationAllowance) { // SlopeLimit!
						transform.position = transform.position + -gravityForce * Time.deltaTime * pushBack +
						                     nearHit.normal * VerticalSkinWidth / 2.0f; // hit.normal here is -ray.direction
					}
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

			WorldMovementProcessed = _charMovement.WorldMovement;
			// Horizontal Collisions
			Vector3 stepOffset = transform.up * StepAllowance;
			/*
			hitsCount = Physics.CapsuleCastNonAlloc(capsuleHead, capsuleFeet + stepOffset, _collider.radius, _charMovement.WorldDir,
				_colliderHits, HorizontalSkinWidth + _speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
			*/

			/*
			RaycastHit[] hitsTemp = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, _charMovement.WorldDir,
				HorizontalSkinWidth + _speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
			int k = 0;
			for (int i = 0; i < hitsTemp.Length; ++i, ++k) {
				_colliderHits[k] = hitsTemp[i];
			} 
			*/

			Vector3 fdir = Vector3.Project(_charMovement.WorldDir, transform.forward);
			RaycastHit[] hitsTemp = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, fdir,
				HorizontalSkinWidth + _speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
			int k = 0;
			for (int i = 0; i < hitsTemp.Length; ++i, ++k) {
				_colliderHits[k] = hitsTemp[i];
			} 
			Debug.DrawRay(transform.position, fdir, Color.red);

			Vector3 rdir = Vector3.Project(_charMovement.WorldDir, transform.right);
			hitsTemp = Physics.CapsuleCastAll(capsuleHead, capsuleFeet + stepOffset, _collider.radius, rdir,
				HorizontalSkinWidth + 1, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < hitsTemp.Length; ++i, ++k) {
				_colliderHits[k] = hitsTemp[i];
			}
			Debug.DrawRay(transform.position, rdir, Color.red);
			hitsCount = k;

			_collisions = (uint) CollisionMask.None;
			for (int i = 0; i < hitsCount; ++i) {
				RaycastHit hit = _colliderHits[i];
				//RepairHitSurfaceNormal(ref hit);
				Debug.DrawRay(hit.point, hit.normal, Color.magenta);
				Debug.Log(hitsCount);
				WorldMovementProcessed = Vector3.ProjectOnPlane(WorldMovementProcessed, hit.normal);
				if (hit.point == Vector3.zero) {
					
					if (Vector3.Dot(hit.normal, Vector3.up) < 1 - SlopeInclinationAllowance) {
						transform.position += hit.normal * (HorizontalSkinWidth / 2.0f); // hit.normal <=> -ray.distance
					}
					
				}
				else {
					if (hit.distance < HorizontalSkinWidth) {
						float dot = Vector3.Dot(transform.forward, hit.normal);
						if (dot < tolerance0) {
							_collisions |= (uint) CollisionMask.Forward;
						}
						dot = Vector3.Dot(-transform.forward, hit.normal);
						if (dot < tolerance0) {
							_collisions |= (uint) CollisionMask.Back;
						}
						dot = Vector3.Dot(transform.right, hit.normal);
						if (dot < tolerance0) {
							_collisions |= (uint) CollisionMask.Right;
						}
						dot = Vector3.Dot(-transform.right, hit.normal);
						if (dot < tolerance0) {
							_collisions |= (uint) CollisionMask.Left;
						}
					}
				}
				
				/*

				float dot = Vector3.Dot(transform.forward, hit.normal);
				if (dot < tolerance0) {
					_collisions |= (uint) CollisionMask.Forward;
				}
				else {
					_collisions &= ~(uint) CollisionMask.Forward;
				}
				dot = Vector3.Dot(-transform.forward, hit.normal);
				if (dot < tolerance0) {
					_collisions |= (uint) CollisionMask.Back;
				}
				else {
					_collisions &= ~(uint) CollisionMask.Back;
				}

				dot = Vector3.Dot(transform.right, hit.normal);
				if (dot < tolerance0) {
					if (dot > tolerance && ((_collisions & (uint)CollisionMask.Forward) > 0) || ((_collisions & (uint)CollisionMask.Back) > 0)) 
						transform.position += -transform.right * Time.deltaTime;
					_collisions |= (uint) CollisionMask.Right;
				}
				else {
					_collisions &= ~(uint) CollisionMask.Right;
				}

				dot = Vector3.Dot(-transform.right, hit.normal);
				if (dot < tolerance0) {
					if (dot > tolerance && ((_collisions & (uint)CollisionMask.Forward) > 0) || ((_collisions & (uint)CollisionMask.Back) > 0)) 
						transform.position += transform.right * Time.deltaTime;
					_collisions |= (uint) CollisionMask.Left;
				}
				else {
					_collisions &= ~(uint) CollisionMask.Left;
				}
				*/
				
			}
			/*
			//Debug.Log(_collisions.ToString("X"));
			if (hitsCount == 0) {
				_collisions = (uint) CollisionMask.None;
			}
			*/
			
		}
		


		public uint GetCollisions() {
			return _collisions;
		}

		private uint CheckWalls(Vector3 capsuleHead, Vector3 capsuleFeet, Vector3 dir, PlayerAction playerActionType) {
			int collidersFound = 0;
			Vector3 stepOffset = transform.up * StepAllowance;
			int wallHitsCount = Physics.CapsuleCastNonAlloc(capsuleHead, capsuleFeet + stepOffset, _collider.radius, dir, _colliderHits,
				4 * HorizontalSkinWidth, _layerMaskAllButPlayer);
			
			Sort(ref _colliderHits, 0, wallHitsCount);
			for (int i = 0; i < wallHitsCount; ++i) {
				RaycastHit hit = _colliderHits[i];
				if (hit.collider.isTrigger) {
					continue;
				}
				++collidersFound;
				if (hit.point == Vector3.zero) {
					_charMovement.AddForce(hit.normal, _charMovement.StepMovement.magnitude + HorizontalSkinWidth / 2.0f);
					break;
				}
				else if (hit.distance > HorizontalSkinWidth / 2.0f && hit.distance < HorizontalSkinWidth) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					_charMovement.AddForce(-dir,
						(1 - Mathf.Abs(Vector3.Dot(Vector3.up, hit.normal))) * ((hit.distance - HorizontalSkinWidth / 2.0f)));
					var action = _actions.GetAction(playerActionType);
					action.Disable();
					break;
				}
			}
			
			/*
			for (int i = 0; i < wallHitsCount; ++i) {
				RaycastHit hit = _colliderHits[i];
				if (hit.collider.isTrigger) {
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
			*/
			if (collidersFound == 0) {
				var action = _actions.GetAction(playerActionType);
				action.Enable();
			}
			
			return 1;
		}

	}
}