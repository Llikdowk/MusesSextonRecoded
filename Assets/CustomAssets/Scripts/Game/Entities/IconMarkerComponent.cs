using UnityEngine;

namespace Game.Entities {
	public class IconMarkerComponent : MonoBehaviour {
		private SpriteRenderer _icon;
		private GameObject _iconGameObject;
		public void Init(string spritePath, Color color) {
			_iconGameObject = new GameObject("_marker");
			_iconGameObject.transform.parent = transform;
			_iconGameObject.transform.LocalReset();
			_iconGameObject.layer = LayerMaskManager.Get(Layer.DrawFront);
			_icon = _iconGameObject.AddComponent<SpriteRenderer>();
			_icon.sprite = Resources.Load<Sprite>(spritePath);
			_icon.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera"));
			_icon.material.color = color;
			_icon.enabled = false;
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
