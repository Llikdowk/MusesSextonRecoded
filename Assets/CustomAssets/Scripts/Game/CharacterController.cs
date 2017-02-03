using Scripts.Debug;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	public Vector3 Gravity = Physics.gravity;
	private CapsuleCollider _collider;
	private Camera _camera;
	private int _layerMaskAllButPlayer;
	private Vector3 _feet;
	public float SkinWidth = 0.1f;
	private float _timeOnAir = 0.0f;

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
	}

	public void Update() {
		Vector3 start = transform.position;
		Vector3 end = start + Vector3.down * 100;
		//Debug.DrawRay(start, end, Color.magenta);

		Matrix4x4 M = _collider.transform.localToWorldMatrix;
		Vector3 p1 = transform.position + M.MultiplyVector(_collider.center) + transform.up * (_collider.height/2.0f - _collider.radius);
		Vector3 p2 = transform.position + M.MultiplyVector(_collider.center) - transform.up * (_collider.height/2.0f - _collider.radius);
		Collider[] colliders = Physics.OverlapCapsule(p1, p2, _collider.radius, _layerMaskAllButPlayer);
		_feet = p2;// - transform.up * _collider.radius;

		//_debugSphere1.transform.position = p1;
		//_debugSphere2.transform.position = p2;

		string s = "";
			Ray raycast = new Ray(_feet, -transform.up);
			Debug.DrawRay(raycast.origin, raycast.direction, Color.magenta);
			Ray raycast2 = new Ray(_feet, transform.forward);
			Debug.DrawRay(raycast2.origin, raycast2.direction, Color.magenta);
		foreach (Collider c in colliders) {
			Debug.DrawRay(raycast.origin, raycast.direction, Color.cyan);
			s += c.name + " ";
			RaycastHit feetRayHit;
			if (Physics.Raycast(raycast, out feetRayHit, 1.0f, _layerMaskAllButPlayer)) {
				if (feetRayHit.distance >= _collider.radius + SkinWidth) {

					Vector3 dif = _feet - feetRayHit.point;
					Vector3 dir = Vector3.up;
					transform.position += Vector3.Min(_collider.radius * dir - dif, dir*(_collider.radius + SkinWidth/2.0f));// * Time.deltaTime * 4.0f;
				}
				else {
					IsGrounded = true;
				}

			}
			else {
				IsGrounded = false;
			}
			/*
				Vector3 closestPoint = c.ClosestPointOnBounds(transform.position);

			Vector3 dif = closestPoint - _feet;
			if (dif.sqrMagnitude < SkinWidth * SkinWidth) {
				IsGrounded = true;
				continue;
			}
			else {
				IsGrounded = false;
			}
			Debug.DrawRay(transform.position, dif, Color.cyan);
			transform.position += dif;// * Time.deltaTime * 4.0f;
			*/
		}
		if (colliders.Length == 0) _isGrounded = false;
		//Debug.Log(colliders.Length + " " + s);

		if (!IsGrounded) {
			transform.localPosition += Gravity * Time.deltaTime;// * _timeOnAir;
		}

		if (!IsGrounded) {
			_timeOnAir = Mathf.Min(_timeOnAir + Time.deltaTime, 4.0f);
		}
	}
}
