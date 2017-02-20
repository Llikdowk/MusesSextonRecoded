
using UnityEngine;

namespace Game.CameraComponents {
	public class UnsaturatePostEffect : MonoBehaviour {

		private Camera _camera;
		private Shader _unsaturateShader;
		private Material _material;
		[Range(0, 1)] public float Intensity;

		public void Awake() {
			_camera = GetComponent<Camera>();
			_unsaturateShader = Shader.Find("Custom/Unsaturate");
			_material = new Material(_unsaturateShader);
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination) {
			_material.SetFloat("_Intensity", Intensity);
			Graphics.Blit(source, destination, _material);
		}

	}
}
