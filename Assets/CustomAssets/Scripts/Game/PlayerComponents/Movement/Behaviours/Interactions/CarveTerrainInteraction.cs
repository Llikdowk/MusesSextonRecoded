using Cubiquity;
using Game.Entities;
using UnityEngine;
using Utils;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public class CarveTerrainInteraction : Interaction {

		private static GameObject _digMarker;

		private readonly CarveTerrainVolumeComponent _terrainCarver;
		private readonly Player _player;

		public CarveTerrainInteraction() {
			if (_digMarker == null) {
				_digMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Object.Destroy(_digMarker.GetComponent<Collider>());
				_digMarker.SetActive(false);
				_digMarker.name = "_digMarker";
				_digMarker.transform.localScale = new Vector3(4, 0.1f, 4);
				_digMarker.layer = LayerMaskManager.Get(Layer.Outline);
				_digMarker.GetComponent<MeshRenderer>().material = new Material(Shader.Find("UI/Default"));
				GameObject marker = new GameObject("_marker");
				marker.transform.parent = _digMarker.transform;
				marker.transform.LocalReset();
				SpriteRenderer sr = marker.AddComponent<SpriteRenderer>();
				sr.sprite = Resources.Load<Sprite>("Sprites/dig");
				sr.material = new Material(Shader.Find("Custom/UniformSpriteFaceCamera"));
				sr.material.color = new Color(0, 81.0f / 255.0f, 240.0f / 255.0f);
			}

			_player = Player.GetInstance();

			string terrainName = "Terrain Volume";
			GameObject terrain = GameObject.Find(terrainName);
			if (!terrain) {
				DebugMsg.GameObjectNotFound(Debug.LogError, terrainName);
			}
			else {
				var terrainVolume = terrain.GetComponent<TerrainVolume>();
				if (!terrainVolume) DebugMsg.ComponentNotFound(Debug.LogError, typeof(TerrainVolume));

				_terrainCarver = terrain.GetComponent<CarveTerrainVolumeComponent>();
				if (!_terrainCarver) DebugMsg.ComponentNotFound(Debug.LogError, typeof(CarveTerrainVolumeComponent));
			}
		}

		// positions are hardcoded as terrain carving is based on fixed coordinates and it is not going to change
		public override void DoInteraction() {
			HideFeedback();

			Vector3[] v = _terrainCarver.DoCarveAction(new Ray(_player.transform.position, _player.Camera.transform.forward));
			Vector3 upperLeft = v[0];
			Vector3 lowerRight = v[1];
			upperLeft = upperLeft + new Vector3(-1, 0, 0);
			lowerRight = lowerRight + new Vector3(0, 0, -1);
			Vector3 position = (lowerRight - upperLeft) / 2.0f + upperLeft;

			GameObject tomb = new GameObject("_tomb");
			TombComponent tombComponent = tomb.AddComponent<TombComponent>();

			RaycastHit hit;
			_player.GetEyeSight(out hit);
			tomb.transform.position = position;
			tomb.transform.up = hit.normal;

			const float offset = 1.75f;
			Vector3 middleUp = new Vector3(lowerRight.x + offset, hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z) / 2.0f);
			tombComponent.AddGravestone(middleUp);

			Vector3 middleLow = new Vector3(upperLeft.x - offset, hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z) / 2.0f);
			Player player = Player.GetInstance();
			player.MoveSmoothlyTo(middleLow, 0.50f);
			AnimationUtils.SlerpTowards(player.transform, player.transform.forward, new Vector3(1, 0, 0), 0.5f);
			player.CurrentState = new DigState(tombComponent.GetGround());
			player.PlayDigAnimation();
		}

		public override void ShowFeedback() {
			RaycastHit hit;
			_player.GetEyeSight(out hit);

			UIUtils.Crosshair(false);
			_digMarker.SetActive(true);
			_digMarker.transform.position = hit.point;
			_digMarker.transform.up = Vector3.Lerp(_digMarker.transform.up, hit.normal, 0.05f);
		}

		public override void HideFeedback() {
			UIUtils.Crosshair(true);
			_digMarker.SetActive(false);
		}

		public override Interaction CheckForPromotion() {
			RaycastHit hit;
			bool hasHit = _player.GetEyeSight(out hit);

			if (!hasHit || hit.distance > 5.0f) return null;
			GameObject g = hit.collider.gameObject;
			if (g.tag == TagManager.Get(Tag.Terrain) && hit.distance > 2.0f) {
				return this;
			}
			return null;
		}
	}
}