
using UnityEngine;

namespace Game.CameraComponents {
	public class AddDrawAlwaysFront : MonoBehaviour {
		private Camera _mainCamera;
		private Camera _alwaysFrontCamera;
		private int _layerMaskDrawFront;
		public float Depth = 100;

		public void Awake() {
			_mainCamera = GetComponent<Camera>();
			_alwaysFrontCamera = new GameObject("AlwaysFrontCamera").AddComponent<Camera>();
			_alwaysFrontCamera.transform.parent = transform;
			_alwaysFrontCamera.transform.localPosition = Vector3.zero;
			_alwaysFrontCamera.transform.eulerAngles = Vector3.zero;
			_layerMaskDrawFront = 1 << LayerMaskManager.Get(Layer.DrawFront);

			_alwaysFrontCamera.CopyFrom(_mainCamera);
			_alwaysFrontCamera.depth = Depth;
			_alwaysFrontCamera.clearFlags = CameraClearFlags.Depth;
			_alwaysFrontCamera.depth = _mainCamera.depth + 1;
			_alwaysFrontCamera.cullingMask = _layerMaskDrawFront;
		}

		public void Update() { // TODO: should be a listener of MainCamera changes
		}

	}
}
