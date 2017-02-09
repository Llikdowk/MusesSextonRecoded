using UnityEngine;

namespace MiscComponents {
	public class MarkableComponent : MonoBehaviour {

		public Color Color = Color.white;
		public Sprite Icon;
		public Vector2 Scale = Vector2.one;

		private GameObject _marker;
		private SpriteRenderer _sprite;

		public void Awake() {
			_marker = new GameObject("_marker");
			_sprite = _marker.AddComponent<SpriteRenderer>();
			_sprite.enabled = false;
		}

		public void Start() {
			_sprite.sprite = Icon;
			_marker.transform.parent = transform;
			_marker.transform.localPosition = Vector3.zero;
			_marker.transform.eulerAngles = Vector3.zero;

			_sprite.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera")) {
				mainTexture = Icon.texture,
				color = Color
			};
			_sprite.material.SetFloat("_ScaleX", Scale.x);
			_sprite.material.SetFloat("_ScaleY", Scale.y);
		}

		public void EnableMark() {
			_sprite.enabled = true;
		}

		public void DisableMark() {
			_sprite.enabled = false;
		}
	}
}