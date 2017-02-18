
using Game.PlayerComponents;
using Triggers;
using UnityEngine;

namespace Game.Entities {
	class TombComponent : MonoBehaviour {

		private GameObject _groundInstantiator;
		private GameObject _ground;
		private SpriteRenderer icon;

		public void Awake() {
			_groundInstantiator = Resources.Load<GameObject>("Prefabs/Ground");
			_groundInstantiator.name = "_ground";
			_groundInstantiator.SetActive(false);

			icon = gameObject.AddComponent<SpriteRenderer>();
			icon.sprite = Resources.Load<Sprite>("Sprites/bury");
			icon.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera")); // TODO extract all shader references
			icon.material.color = new Color(0, 81.0f/255.0f, 240.0f/255.0f);
			icon.enabled = false;

			_ground = Object.Instantiate(_groundInstantiator);
			_ground.transform.parent = transform;
			_ground.transform.LocalReset();
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
			icon.enabled = show;
		}

		public void Start() {
			
		}

	}
}
