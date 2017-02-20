using UnityEngine;

namespace Game.Entities {
	public class IconMarkerComponent : MonoBehaviour {
		private SpriteRenderer _icon;
		private GameObject _iconGameObject;
		public void Init(string spritePath) {
			_iconGameObject = new GameObject("_marker");
			_iconGameObject.transform.parent = transform;
			_iconGameObject.transform.LocalReset();
			_iconGameObject.layer = LayerMaskManager.Get(Layer.DrawFront);
			_icon = _iconGameObject.AddComponent<SpriteRenderer>();
			_icon.sprite = Resources.Load<Sprite>("Sprites/bury");
			_icon.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera")); // TODO extract all shader references
			_icon.material.color = new Color(0, 81.0f/255.0f, 240.0f/255.0f);
			_icon.enabled = false;
		}

		public void OnEnable() {
			Show(true);
		}

		public void OnDisable() {
			Show(false);
		}

		public void Show(bool enabled) {
			if (_icon) { _icon.enabled = enabled; }
		}
	}
}
