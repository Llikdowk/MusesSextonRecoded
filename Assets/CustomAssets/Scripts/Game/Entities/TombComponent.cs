﻿
using Game.PlayerComponents;
using Triggers;
using UnityEngine;
using Utils;

namespace Game.Entities {
	public class TombComponent : MonoBehaviour {
		private static GameObject _groundInstantiator;
		private static GameObject _groundHeapInstantiator;

		public Vector3 MiddleLow { get; private set; }

		private GameObject _gravestone;
		private GameObject _ground;
		private GameObject _groundHeap;
		private IconMarkerComponent _icon;

		private const float _downGroundStep = 1.0f;
		private const float _upGroundStep = 0.6f;
		private const float _heapStep = 0.5f;
		private const float _upGravestoneStep = 1.5f;

		public void Init(Vector3 upperLeft, Vector3 lowerRight) {

			if (!_groundInstantiator) {
				_groundInstantiator = Resources.Load<GameObject>("Prefabs/Ground");
				_groundInstantiator.name = "_ground";
				_groundInstantiator.SetActive(false);
			}
			if (!_groundHeapInstantiator) {
				_groundHeapInstantiator = Resources.Load<GameObject>("Prefabs/GroundHeap");
				_groundHeapInstantiator.name = "_groundHeap";
				_groundHeapInstantiator.SetActive(false);
			}

			_icon = gameObject.AddComponent<IconMarkerComponent>();
			_icon.Init("Sprites/bury", new Color(0, 81.0f/255.0f, 240.0f/255.0f));

			_ground = Object.Instantiate(_groundInstantiator);
			_ground.transform.parent = transform;
			_ground.transform.LocalReset();
			_ground.transform.localPosition = Vector3.down * 0.5f;
			_ground.SetActive(true);

			_groundHeap = Object.Instantiate(_groundHeapInstantiator);
			_groundHeap.transform.parent = transform;
			_groundHeap.transform.LocalReset();
			_groundHeap.transform.up = Vector3.up;
			_groundHeap.transform.localPosition = new Vector3(-2, -1.25f, -3.5f);
			_groundHeap.SetActive(true);

			SphereCollider c = gameObject.AddComponent<SphereCollider>();
			c.isTrigger = false;
			c.radius = 2.75f;
			c.center = new Vector3(0, 0.85f, 0);
			
			SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
			trigger.isTrigger = true;
			trigger.radius = 4.5f;

			TTomb triggerTomb = gameObject.AddComponent<TTomb>();
			triggerTomb.Init(this);

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

		public void ShowActionIcon(bool show) {
			_icon.enabled = show;
		}

		public void Dig() {
			_ground.transform.position -= _ground.transform.up * _downGroundStep;
			_groundHeap.transform.position += _ground.transform.up * _heapStep;
		}

		public void Bury() {
			_ground.transform.position += _ground.transform.up * _upGroundStep;
			_groundHeap.transform.position -= _ground.transform.up * _heapStep;
			_gravestone.transform.position += _gravestone.transform.up * _upGravestoneStep;
		}

		private void AddGravestone(Vector3 position) {
			GameObject gravestone = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tombstone"));
			gravestone.transform.parent = transform;
			gravestone.transform.LocalReset();
			gravestone.transform.position = position;
			gravestone.transform.localPosition -= Vector3.up * 3.0f;
			_gravestone = gravestone;
		}

		public void PlayerTombTransition(PlayerState newPlayerState, bool animate) {
			Player player = Player.GetInstance();
			player.MoveSmoothlyTo(MiddleLow, 0.50f);
			if (animate) {
				player.PlayDigAnimation();
			}
			AnimationUtils.SlerpTowards(player.transform, player.transform.forward, new Vector3(1, 0, 0), 0.5f, () => {
				player.CurrentState = newPlayerState;
			});
		}

		public void Hide() {
			_ground.transform.parent = null;
			_gravestone.transform.parent = null;
			_groundHeap.transform.parent = null;
			gameObject.SetActive(false);
		}

	}
}
