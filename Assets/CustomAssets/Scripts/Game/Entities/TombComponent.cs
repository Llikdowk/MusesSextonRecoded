
using Game.PlayerComponents;
using Triggers;
using UnityEngine;

namespace Game.Entities {
	class TombComponent : MonoBehaviour {

		public GameObject Gravestone { get { return _gravestone; } }
		private GameObject _gravestone;
		private GameObject _groundInstantiator;
		private GameObject _ground;
		private IconMarkerComponent _icon;


		public void Awake() {

			_groundInstantiator = Resources.Load<GameObject>("Prefabs/Ground");
			_groundInstantiator.name = "_ground";
			_groundInstantiator.SetActive(false);

			_icon = gameObject.AddComponent<IconMarkerComponent>();
			_icon.Init("Sprites/bury", new Color(0, 81.0f/255.0f, 240.0f/255.0f));

			_ground = Object.Instantiate(_groundInstantiator);
			_ground.transform.parent = transform;
			_ground.transform.LocalReset();
			_ground.transform.localPosition = Vector3.down * 0.5f;
			_ground.SetActive(true);

			SphereCollider c = gameObject.AddComponent<SphereCollider>();
			c.isTrigger = false;
			c.radius = 2.75f;
			c.center = new Vector3(0, 0.85f, 0);
			
			SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
			trigger.isTrigger = true;
			trigger.radius = 4.5f;

			TTomb triggerTomb = gameObject.AddComponent<TTomb>();
			triggerTomb.Init(_ground);
		}

		public GameObject GetGround() {
			return _ground;
		}

		public void ShowActionIcon(bool show) {
			_icon.enabled = show;
		}

		public void AddGravestone(Vector3 position) {
			GameObject gravestone = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tombstone"));
			gravestone.transform.parent = transform;
			gravestone.transform.LocalReset();
			gravestone.transform.position = position;
			gravestone.transform.localPosition -= Vector3.up * 3.0f;
			_gravestone = gravestone;
		}

		public void Start() {
			
		}

	}
}
