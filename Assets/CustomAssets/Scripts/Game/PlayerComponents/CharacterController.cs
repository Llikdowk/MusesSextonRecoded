using UnityEngine;

namespace Game.PlayerComponents {
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
		[Range(-1, 0)] public float NormalTolerance = -0.65f;
		public bool InteractsWithTriggers = true;

		private CapsuleCollider _collider;
		private int _layerMaskAllButPlayer;
		private float _timeOnAir = 0.0f;
		private CharacterMovement _charMovement;

		private bool _isGrounded = false;

		private const int MaxSimultaneousColliders = 32;
		private RaycastHit[] _colliderHits = new RaycastHit[MaxSimultaneousColliders];
		private C5.IList<Collider> _currentTriggers = new C5.ArrayList<Collider>(); 
		private readonly C5.IList<Collider> _stayTriggers = new C5.ArrayList<Collider>();

		public void Start() {
			int player = 1 << LayerMaskManager.Get(Layer.Player);
			_layerMaskAllButPlayer = ~player;
			_collider = GetComponent<CapsuleCollider>();
			_charMovement = GetComponent<CharacterMovement>();

		}

		private void ProcessTriggers(ref RaycastHit[] hits, ref int hitsLength) {
			_currentTriggers = _currentTriggers.View(0, 0);

			for (int i = hitsLength-1; i >= 0; --i) {
				if (hits[i].collider.isTrigger) {
					if (hits[i].distance == 0) {
						_currentTriggers.Add(hits[i].collider);
					}
					for (int j = i; j < hitsLength - 1; ++j) {
						hits[i] = hits[i + 1];
					}
					--hitsLength;
				}
			}

			if (InteractsWithTriggers) {
				foreach (Collider collider in _currentTriggers) {
					if (_stayTriggers.Contains(collider)) {
						collider.SendMessage("OnTriggerStay", _collider, SendMessageOptions.DontRequireReceiver);
					}
					else {
						collider.SendMessage("OnTriggerEnter", _collider, SendMessageOptions.DontRequireReceiver);
						_stayTriggers.Add(collider);
					}
				}

				for (int i = _stayTriggers.Count - 1; i >= 0; --i) {
					Collider collider = _stayTriggers[i];
					if (!_currentTriggers.Contains(collider)) {
						collider.SendMessage("OnTriggerExit", _collider, SendMessageOptions.DontRequireReceiver);
						_stayTriggers.Remove(collider);
					}
				}
			}
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

		private Vector3 _lastPosition;
		public Vector3 WorldMovementProcessed = Vector3.zero; // TODO: encapsulate this
		public void Update() {
			float speed = (transform.position - _lastPosition).magnitude;
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
				_colliderHits, GrounderDistance, _layerMaskAllButPlayer, QueryTriggerInteraction.Collide);
			ProcessTriggers(ref _colliderHits, ref hitsCount);

			Vector3 floorNormal = Vector3.up;
			if (hitsCount > 0) {
				Sort(ref _colliderHits, 0, hitsCount);
				RaycastHit nearHit = _colliderHits[0];
				floorNormal = nearHit.normal;
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

			// Horizontal Collisions
			WorldMovementProcessed = _charMovement.WorldMovement;
			Vector3 stepOffset = transform.up * StepAllowance;
			
			hitsCount = Physics.CapsuleCastNonAlloc(capsuleHead, capsuleFeet + stepOffset, _collider.radius, _charMovement.WorldDir,
				_colliderHits, HorizontalSkinWidth + speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore);

			for (int i = 0; i < hitsCount; ++i) {
				RaycastHit hit = _colliderHits[i];
				
				Debug.DrawRay(hit.point, hit.normal, Color.magenta);
				WorldMovementProcessed = Vector3.ProjectOnPlane(WorldMovementProcessed, hit.normal);
				
				RaycastHit innerHit;
				Vector3 dir = WorldMovementProcessed;
				if (transform.worldToLocalMatrix.MultiplyVector(Vector3.Project(floorNormal, WorldMovementProcessed)).z < 0) { // slope up
					dir = Vector3.ProjectOnPlane(WorldMovementProcessed, floorNormal);
				}
				Debug.DrawRay(hit.point, dir, Color.red);
				if (Physics.CapsuleCast(capsuleHead, capsuleFeet, _collider.radius, dir, out innerHit,
					HorizontalSkinWidth + speed, _layerMaskAllButPlayer, QueryTriggerInteraction.Ignore)) 
				{
					Debug.DrawRay(innerHit.point, innerHit.normal, Color.magenta);
					WorldMovementProcessed = Vector3.zero;
					break;
				}
				
			}

		}
	}
}