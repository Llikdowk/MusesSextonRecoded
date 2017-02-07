using UnityEngine;

public class OutlinerPosteffect : MonoBehaviour {
	public Color OutlineColor = Color.cyan;

    private Shader _outlineShader;
	private Shader _drawSimpleShader;
	private Material _material;
	private Camera _camera;
	private Camera _outlinerCamera;
	private int _layerMaskOnlyOutline;
 
    public void Awake() {
	    _outlineShader = Shader.Find("Custom/PostOutline");
	    _drawSimpleShader = Shader.Find("Custom/DrawSimple");
        _camera = GetComponent<Camera>();
		_outlinerCamera = new GameObject("CameraOutliner").AddComponent<Camera>();
	    _outlinerCamera.enabled = false;
	    _outlinerCamera.transform.parent = transform;
	    _layerMaskOnlyOutline = 1 << LayerMaskManager.Get(Layer.Outline);
		_material = new Material(_outlineShader);
	    _material.color = OutlineColor;
    }
 
    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        _outlinerCamera.CopyFrom(_camera);
        _outlinerCamera.clearFlags = CameraClearFlags.Color;
        _outlinerCamera.backgroundColor = Color.black;
	    _outlinerCamera.cullingMask = _layerMaskOnlyOutline;
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.R8);
        _outlinerCamera.targetTexture = rt;
        _outlinerCamera.RenderWithShader(_drawSimpleShader, "");
        Graphics.Blit(rt, destination, _material);
        RenderTexture.ReleaseTemporary(rt);
    }
}
