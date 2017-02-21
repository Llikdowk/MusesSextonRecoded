
using Game.PlayerComponents;
using Triggers;
using UnityEngine;
using Utils;

namespace Game.Entities {
	class TombComponent : MonoBehaviour {

		public GameObject Gravestone { get { return _gravestone; } }
		public Vector3 MiddleLow { get; private set; }

		private GameObject _gravestone;
		private GameObject _groundInstantiator;
		private GameObject _ground;
		private IconMarkerComponent _icon;


		public void Init(Vector3 upperLeft, Vector3 lowerRight) {

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


			Vector3 position = (lowerRight - upperLeft) / 2.0f + upperLeft;
			RaycastHit hit;
			Player.GetInstance().GetEyeSight(out hit);
			transform.position = position;
			transform.up = hit.normal;

			const float offset = 1.75f;
			Vector3 middleUp = new Vector3(lowerRight.x + offset, hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z) / 2.0f);
			AddGravestone(middleUp);
			MiddleLow = new Vector3(upperLeft.x - offset, hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z) / 2.0f);
		}

		public GameObject GetGround() {
			return _ground;
		}

		public void ShowActionIcon(bool show) {
			_icon.enabled = show;
		}


		private void AddGravestone(Vector3 position) {
			GameObject gravestone = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tombstone"));
			gravestone.transform.parent = transform;
			gravestone.transform.LocalReset();
			gravestone.transform.position = position;
			gravestone.transform.localPosition -= Vector3.up * 3.0f;
			_gravestone = gravestone;
		}

		public void PlayerTombTransition(PlayerState newPlayerState) {
			Player player = Player.GetInstance();
			player.MoveSmoothlyTo(MiddleLow, 0.50f);
			player.PlayDigAnimation();
			AnimationUtils.SlerpTowards(player.transform, player.transform.forward, new Vector3(1, 0, 0), 0.5f, () => {
				player.CurrentState = newPlayerState;
			});
		}

	}
}
