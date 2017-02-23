using UnityEngine;

namespace Game.Entities {
	public class IconMarkerComponent : MonoBehaviour {
		private static readonly C5.ArrayList<IconMarkerComponent> AllVisibleMarkersInScene = new C5.ArrayList<IconMarkerComponent>();
		private SpriteRenderer _icon;
		private GameObject _iconGameObject;

		public static void HideAll() {
			for (int i = AllVisibleMarkersInScene.Count - 1; i >= 0; --i) {
				if (AllVisibleMarkersInScene[i].gameObject.activeSelf) {
					AllVisibleMarkersInScene[i].Show(false);
				}
			}
		}

		public static void ShowAll() {
			for (int i = AllVisibleMarkersInScene.Count - 1; i >= 0; --i) {
				if (AllVisibleMarkersInScene[i].gameObject.activeSelf) {
					AllVisibleMarkersInScene[i].Show(true);
				}
			}
		}

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
			AllVisibleMarkersInScene.Add(this);
			Show(true);
		}

		public void OnDisable() {
			AllVisibleMarkersInScene.Remove(this);
			Show(false);
		}

		private void Show(bool show) {
			if (_icon) {
				_icon.enabled = show;
			}
		}
	}
}
