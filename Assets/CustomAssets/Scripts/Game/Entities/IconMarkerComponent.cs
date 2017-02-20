using UnityEngine;

namespace Game.Entities {
	public class IconMarkerComponent : MonoBehaviour {
		private SpriteRenderer _icon;
		private GameObject _iconGameObject;

		private void BaseInit() {
			_iconGameObject = new GameObject("_marker");
			_iconGameObject.transform.parent = transform;
			_iconGameObject.transform.LocalReset();
			_iconGameObject.layer = LayerMaskManager.Get(Layer.DrawFront);
			_icon = _iconGameObject.AddComponent<SpriteRenderer>();
		}

		public void Init(Sprite sprite, Color color) {
			BaseInit();
			_icon.sprite = sprite;
			_icon.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera"));
			_icon.material.color = color;
			_icon.enabled = false;
		}

		public void Init(string spritePath, Color color) {
			BaseInit();
			_icon.sprite = Resources.Load<Sprite>(spritePath);
			_icon.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera"));
			_icon.material.color = color;
			_icon.enabled = false;
		}

		public void SetScaleX(float scale) {
			_icon.material.SetFloat("_ScaleX", scale);
		}

		public void SetScaleY(float scale) {
			_icon.material.SetFloat("_ScaleY", scale);
		}

		public void OnEnable() {
			Show(true);
		}

		public void OnDisable() {
			Show(false);
		}

		public void Show(bool show) {
			if (_icon) { _icon.enabled = show; }
		}
	}
}
