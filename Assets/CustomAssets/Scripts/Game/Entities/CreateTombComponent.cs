
using Game.PlayerComponents;
using Triggers;
using UnityEngine;

namespace Game.Entities {
	class CreateTombComponent : MonoBehaviour {

		private GameObject _groundInstantiator;
		private GameObject _ground;

		public void Awake() {
			_groundInstantiator = Resources.Load<GameObject>("Prefabs/Ground");
			_groundInstantiator.name = "_ground";
			_groundInstantiator.SetActive(false);

			SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
			sr.sprite = Resources.Load<Sprite>("Sprites/bury");
			sr.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera")); // TODO extract all shader references
			sr.material.color = new Color(0, 81.0f/255.0f, 240.0f/255.0f);
			sr.enabled = false;

			_ground = Object.Instantiate(_groundInstantiator);
			_ground.transform.parent = transform;
			_ground.transform.LocalReset();
			_ground.SetActive(true);

			SphereCollider c = gameObject.AddComponent<SphereCollider>();
			c.isTrigger = false;
			c.radius = 3.85f;
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

		public void Start() {
			
		}

	}
}
