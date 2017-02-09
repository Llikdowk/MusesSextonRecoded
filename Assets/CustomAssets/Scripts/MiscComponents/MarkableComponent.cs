using UnityEngine;

namespace MiscComponents {
	public class MarkableComponent : MonoBehaviour {

		public Color Color = Color.white;
		public Sprite Icon;
		public Vector2 Scale = Vector2.one;
		public Vector3 Offset = Vector3.zero;
		public bool StartEnabled = false;

		private GameObject _marker;
		private SpriteRenderer _sprite;

		public void Awake() {
			_marker = new GameObject("_marker");
			_sprite = _marker.AddComponent<SpriteRenderer>();
			_sprite.enabled = StartEnabled;
		}

		public void Start() {
			_sprite.sprite = Icon;
			_marker.transform.parent = transform;
			_marker.transform.localPosition = Offset;
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