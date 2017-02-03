using Scripts.Debug;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	public Vector3 Gravity = Physics.gravity;
	private CapsuleCollider _collider;
	private Camera _camera;
	private Transform _transform;
	private int _layerMaskAllButPlayer;

	private GameObject _debugSphere1;
	private GameObject _debugSphere2;


	public void Start() {
		_debugSphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		_debugSphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Object.Destroy(_debugSphere1.GetComponent<Collider>());
		Object.Destroy(_debugSphere2.GetComponent<Collider>());
		int player = 1 << LayerMaskManager.GetInstance().GetLayer(Layer.Player);
		_layerMaskAllButPlayer = ~player;
		_camera = GetComponentInChildren<Camera>();
		_collider = GetComponent<CapsuleCollider>();
		_transform = GetComponent<Transform>();
	}

	public void Update() {
		Vector3 start = _transform.position;
		Vector3 end = start + Vector3.down * 100;
		Debug.DrawRay(start, end, Color.magenta);

		Matrix4x4 M = _collider.transform.localToWorldMatrix;
		Vector3 p1 = _transform.position + M.MultiplyVector(_collider.center) + transform.up * (_collider.height/2.0f - _collider.radius);
		Vector3 p2 = _transform.position + M.MultiplyVector(_collider.center) - transform.up * (_collider.height/2.0f - _collider.radius);
		Collider[] colliders = Physics.OverlapCapsule(p1, p2, _collider.radius, _layerMaskAllButPlayer);

		_debugSphere1.transform.position = p1;
		_debugSphere2.transform.position = p2;

		string s = "";
		foreach (Collider c in colliders) {
			s += c.name + " ";
		}

		Debug.Log(colliders.Length + " " + s);


		//DebugDraw.RenderVolume(Vector3.zero, Vector3.one, 2, Vector3.down, 10);
		//Physics.OverlapCapsule();
	}
}
