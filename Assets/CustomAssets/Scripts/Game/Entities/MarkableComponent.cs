using Game.Entities;
using UnityEngine;

namespace MiscComponents {
	public class MarkableComponent : MonoBehaviour {

		public Color Color = Color.white;
		public Sprite Icon;
		public Vector2 Scale = Vector2.one;
		public Vector3 Offset = Vector3.zero;
		public bool StartEnabled = false;
		private IconMarkerComponent _icon;



		public void Awake() {
			GameObject g = new GameObject("_marker");
			g.transform.parent = transform;
			g.transform.LocalReset();
			g.transform.localPosition += Offset;
			_icon = g.AddComponent<IconMarkerComponent>();
			_icon.Init(Icon, Color);
			_icon.SetScaleX(Scale.x);
			_icon.SetScaleY(Scale.y);
			enabled = StartEnabled;
		}

		public void OnEnable() {
			_icon.enabled = true;
		}

		public void OnDisable() {
			_icon.enabled = false;
		}
	}
}