using Game;
using Scripts.Debug;
using UnityEditor;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	public Vector3 Gravity = Physics.gravity;
	[Range(0, 0.2f)]
	public float SkinWidth = 0.15f;
	public float ForwardSpeed = 1.0f;
	public float BackwardSpeed = 1.0f;
	public float LeftSpeed = 1.0f;
	public float RightSpeed = 1.0f;

	private CapsuleCollider _collider;
	private Camera _camera;
	private int _layerMaskAllButPlayer;
	private Vector3 _feet;
	private float _timeOnAir = 0.0f;
	private Vector3 _lastPosition;
	private Vector3 _currentSpeed; // units/s (m/s)

	private bool IsGrounded {
		get { return _isGrounded; }
		set {
			_isGrounded = value;
			if (_isGrounded) {
				_timeOnAir = 0.0f;
			}
		}
	}

	private bool _isGrounded; //= false;

	//private GameObject _debugSphere1;
	//private GameObject _debugSphere2;


	public void Start() {
		//_debugSphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//_debugSphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//Object.Destroy(_debugSphere1.GetComponent<Collider>());
		//Object.Destroy(_debugSphere2.GetComponent<Collider>());
		int player = 1 << LayerMaskManager.GetInstance().GetLayer(Layer.Player);
		_layerMaskAllButPlayer = ~player;
		_camera = GetComponentInChildren<Camera>();
		_collider = GetComponent<CapsuleCollider>();
		ActionManager actions = ActionManager.GetInstance();

		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		actions.GetAction(ActionTag.MoveForward).WhileBehaviour = () => {
			transform.position += transform.forward * ForwardSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveBack).WhileBehaviour = () => {
			transform.position -= transform.forward * BackwardSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveLeft).WhileBehaviour = () => {
			transform.position -= transform.right * LeftSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveRight).WhileBehaviour = () => {
			transform.position += transform.right * RightSpeed * Time.deltaTime;
		};
	}

	public void Update() {
		
		_currentSpeed = (transform.position - _lastPosition) / Time.deltaTime;
		_lastPosition = transform.position;
		if (!IsGrounded) {
			transform.position += Gravity * /*_timeOnAir */ Time.deltaTime;
		}

		Matrix4x4 M = _collider.transform.localToWorldMatrix;
		Vector3 p1 = transform.position + M.MultiplyVector(_collider.center) + transform.up * (_collider.height/2.0f - _collider.radius);
		Vector3 p2 = transform.position + M.MultiplyVector(_collider.center) - transform.up * (_collider.height/2.0f - _collider.radius);
		Collider[] colliders = Physics.OverlapCapsule(p1, p2, _collider.radius, _layerMaskAllButPlayer);
		_feet = p2;// - transform.up * _collider.radius;

		//_debugSphere1.transform.position = p1;
		//_debugSphere2.transform.position = p2;

		string s = "";
		float raycastMargin = 0.1f;
		Ray raycast = new Ray(_feet + raycastMargin * transform.up, -transform.up);
		Debug.DrawRay(raycast.origin, raycast.direction, Color.magenta);

		Ray[] wallRaycast = new Ray[8];
		wallRaycast[0] = new Ray(_feet, transform.forward);
		wallRaycast[1] = new Ray(_feet, -transform.forward);
		wallRaycast[2] = new Ray(_feet, transform.right);
		wallRaycast[3] = new Ray(_feet, -transform.right);

		// fixme disposable hack: not working properly and poorly optimized
		wallRaycast[4] = new Ray(_feet, transform.forward + transform.right);
		wallRaycast[5] = new Ray(_feet, -transform.forward + transform.right);
		wallRaycast[6] = new Ray(_feet, -transform.right + transform.forward);
		wallRaycast[7] = new Ray(_feet, -transform.right - transform.forward);
		foreach (Ray r in wallRaycast) {
			Debug.DrawRay(r.origin, r.direction, Color.magenta);
		}

		RaycastHit hit;
		foreach (Collider collider in colliders) {
			s += collider.name + " ";

			if (collider.Raycast(raycast, out hit, 10.0f)) {
			Debug.DrawRay(transform.position, collider.ClosestPointOnBounds(hit.point) - transform.position, Color.red);
				if (hit.distance <= SkinWidth) continue;
				Debug.DrawRay(raycast.origin, raycast.direction, Color.cyan);
				Vector3 dir = -raycast.direction;
				Vector3 dif = raycast.origin - hit.point;
				if (hit.distance > _collider.radius + SkinWidth) { 
					transform.position += _collider.radius * dir - dif; 
				}
				else {
					IsGrounded = true;
					transform.position += (_collider.radius + SkinWidth/2.0f) * dir - dif;

				}
			}

			foreach (Ray wallRay in wallRaycast) {
				if (collider.Raycast(wallRay, out hit, 1.0f)) {
					if (hit.distance <= SkinWidth) continue;
					Debug.DrawRay(wallRay.origin, wallRay.direction, Color.cyan);
					if (hit.distance <= _collider.radius) {
						Vector3 dir = -wallRay.direction;
						Vector3 dif = wallRay.origin - hit.point;
						transform.position += (_collider.radius + SkinWidth / 2.0f) * dir - dif;
					}
				}
			}

	
		}
		
		if (colliders.Length == 0) _isGrounded = false;

		if (!IsGrounded) {
			_timeOnAir += Time.deltaTime;
		}
	}

}
