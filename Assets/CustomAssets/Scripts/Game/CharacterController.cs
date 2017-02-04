using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Game;
using Scripts.Debug;
using UnityEditor;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	public Vector3 Gravity = Physics.gravity;
	public float GravityMultiplier = 1.0f;
	[Range(0, 0.5f)] public float SkinWidth = 0.15f;
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
	private Vector3 _nextRelPosition = Vector3.zero;
	private Vector3 _movementDirection = Vector3.zero;
	private float _pushbackForce = 1.5f;

	private bool IsGrounded {
		get { return _isGrounded; }
		set {
			_isGrounded = value;
			if (_isGrounded) {
				_timeOnAir = 0.0f;
			}
		}
	}

	private bool _isGrounded = false;

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

		actions.GetAction(ActionTag.MoveForward).WhileBehaviour = () => {
			_nextRelPosition += transform.forward * ForwardSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveBack).WhileBehaviour = () => {
			_nextRelPosition -= transform.forward * BackwardSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveLeft).WhileBehaviour = () => {
			_nextRelPosition -= transform.right * LeftSpeed * Time.deltaTime;
		};
		actions.GetAction(ActionTag.MoveRight).WhileBehaviour = () => {
			_nextRelPosition += transform.right * RightSpeed * Time.deltaTime;
		};
		Gravity *= GravityMultiplier;
	}


	public void Update() {
		_currentSpeed = _nextRelPosition / Time.deltaTime;
		_movementDirection = _nextRelPosition.normalized;
		transform.position += _nextRelPosition;
		_nextRelPosition = Vector3.zero;
		if (!IsGrounded) {
			_nextRelPosition += Gravity * _timeOnAir * Time.deltaTime;
		}
		Matrix4x4 M = _collider.transform.localToWorldMatrix;
		Vector3 p1 = transform.position + M.MultiplyVector(_collider.center) +
		             transform.up * (_collider.height / 2.0f - _collider.radius);
		Vector3 p2 = transform.position + M.MultiplyVector(_collider.center) -
		             transform.up * (_collider.height / 2.0f - _collider.radius);
		RaycastHit[] floorHits = Physics.CapsuleCastAll(p1, p2, _collider.radius, -Vector3.up, SkinWidth, _layerMaskAllButPlayer);
		if (floorHits.Length == 0) IsGrounded = false;

		foreach (RaycastHit hit in floorHits) {
			// overlapping collision
			if (hit.point == Vector3.zero) {
				transform.position += hit.normal * (_nextRelPosition.magnitude) + hit.normal * SkinWidth / 2.0f;
				IsGrounded = true;
			}
			else {
				Debug.Log("distance: " + hit.distance);
				if (hit.distance > SkinWidth / 2.0f) {
					Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
					transform.position += (hit.distance - SkinWidth/2.0f) * -Vector3.up; //(transform.position - hit.point);
					IsGrounded = true;
				}
			}
		}

		if (!IsGrounded) {
			_timeOnAir += Time.deltaTime;
		}


		RaycastHit[] wallHits = Physics.CapsuleCastAll(p1, p2, _collider.radius, Vector3.ProjectOnPlane(_movementDirection, Vector3.up)/*transform.forward*/, SkinWidth, _layerMaskAllButPlayer);
		foreach (RaycastHit hit in wallHits) {
			if (hit.point == Vector3.zero) {
				transform.position += (hit.distance - SkinWidth/2.0f * 10.0f) * -hit.normal; //(transform.position - hit.point);
			}
			else { // if (hit.distance > SkinWidth / 2.0f) {
				Debug.DrawRay(transform.position, hit.point - transform.position, Color.magenta);
				transform.position += (hit.distance - SkinWidth/2.0f * 10.0f) * -hit.normal; //(transform.position - hit.point);
			}
		}

	}

}
