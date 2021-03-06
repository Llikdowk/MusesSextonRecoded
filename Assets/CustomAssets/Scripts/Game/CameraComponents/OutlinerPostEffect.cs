﻿using UnityEngine;

namespace Game.CameraComponents {
	public class OutlinerPostEffect : MonoBehaviour {
		public Color OutlineColor = Color.cyan;
		public int Thickness = 6;

		private Shader _outlineShader;
		private Shader _drawSimpleShader;
		private Material _material;
		private Camera _mainCamera;
		private Camera _outlinerCamera;
		private int _layerMaskOnlyOutline;

		
		public void Awake() {
			
			_outlineShader = Shader.Find("Custom/PostOutline");
			_drawSimpleShader = Shader.Find("Custom/DrawSimple");
			_mainCamera = GetComponent<Camera>();
			_outlinerCamera = new GameObject("CameraOutliner").AddComponent<Camera>();
			_outlinerCamera.enabled = false;
			_outlinerCamera.transform.parent = transform;
			_outlinerCamera.transform.LocalReset();
			_layerMaskOnlyOutline = 1 << LayerMaskManager.Get(Layer.Outline);
			_material = new Material(_outlineShader) {color = OutlineColor};
			_material.SetInt("_Thickness", Thickness);
			
			_outlinerCamera.CopyFrom(_mainCamera);
			_outlinerCamera.clearFlags = CameraClearFlags.Color; // TODO: these params should be changed based on a MainCamera listener, not per frame
			_outlinerCamera.backgroundColor = Color.black;
			_outlinerCamera.cullingMask = _layerMaskOnlyOutline;

		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination) {

			RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);
			_outlinerCamera.targetTexture = rt;
			_outlinerCamera.RenderWithShader(_drawSimpleShader, "");
			Graphics.Blit(rt, destination, _material);
			RenderTexture.ReleaseTemporary(rt);
		}
	}
}