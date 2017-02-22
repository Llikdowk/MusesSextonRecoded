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
				IconMarkerComponent icon = _digMarker.AddComponent<IconMarkerComponent>();
				icon.Init("Sprites/dig", new Color(0, 81.0f / 255.0f, 240.0f / 255.0f));
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

			Vector3[] v = _terrainCarver.DoCarveAction(new Ray(_player.transform.position, _player.MainCamera.transform.forward));
			Vector3 upperLeft = v[0];
			Vector3 lowerRight = v[1];
			upperLeft = upperLeft + new Vector3(-1, 0, 0);
			lowerRight = lowerRight + new Vector3(0, 0, -1);

			GameObject tomb = new GameObject("_tomb");
			TombComponent tombComponent = tomb.AddComponent<TombComponent>();
			tombComponent.Init(upperLeft, lowerRight);
			tombComponent.PlayerTombTransition(new DigState(tombComponent), true);
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
				if (Vector3.Dot(Vector3.up, hit.normal) > 0.92f) {
					if (PassHeightRestrictions(hit.point, hit.normal)) {
						return this;
					}
				}
			}
			return null;
		}

		public bool PassHeightRestrictions(Vector3 point, Vector3 normal) {
			RaycastHit[] hits = Physics.BoxCastAll(point + Vector3.up*5.0f, _digMarker.transform.localScale, Vector3.down,
				Quaternion.identity, 10.0f, ~(1 << LayerMaskManager.Get(Layer.Player)), QueryTriggerInteraction.Collide);
			if (hits.Length == 0) {
				return true;
			}

			float maxHeight = -99;
			float minHeight = 99;

			foreach (RaycastHit hit in hits) {
				if (hit.collider.gameObject.tag != TagManager.Get(Tag.Terrain)) {
					return false;
				}
				if (hit.distance == 0.0f) {
					return false;
				}
				if (hit.distance > maxHeight) {
					maxHeight = hit.distance;
				}
				if (hit.distance < minHeight) {
					minHeight = hit.distance;
				}
			}
			if (maxHeight - minHeight > 1.0f) {
				return false;
			}
			return true;
		}
	}
}