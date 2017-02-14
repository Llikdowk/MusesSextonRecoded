
using Cubiquity;
using Game.Entities;
using MiscComponents;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {
	public abstract class Interaction {
		public abstract void DoInteraction();

		/* TODO?
		public void CheckForInteraction() {
			
		}
		*/

		public virtual void ShowFeedback() {
		}

		public virtual void HideFeedback() {
		}

		//public abstract void OnInteractionFinished
	}

	public class EmptyInteraction : Interaction {
		public override void DoInteraction() {
			Debug.Log("DO EMPTY INTERACTION");
		}
	}

	public class UseCartInteraction : Interaction {
		public override void DoInteraction() {
			
		}
		
	}

	
	public class PickUpCoffinInteraction : Interaction {
		public GameObject Coffin;
		private readonly int _backupLayer;

		public PickUpCoffinInteraction(GameObject coffin) {
			Debug.Log("CONSTRUCT COFFIN INTERACTION");
			Coffin = coffin;
			_backupLayer = coffin.layer;
		}

		public override void DoInteraction() {
			Debug.Log("DO PICK UP INTERACTION");
			Coffin.layer = LayerMaskManager.Get(Layer.DrawFront);
			MarkableComponent m = Coffin.GetComponent<MarkableComponent>();
			if (m != null) {
				m.DisableMark();
			} else {
				DebugMsg.ComponentNotFound(Debug.LogWarning, typeof(MarkableComponent));
			}
			Player.GetInstance().CurrentState = new DragCoffinState(Coffin);
		}

		public override void ShowFeedback() {
			Coffin.layer = LayerMaskManager.Get(Layer.Outline);
		}

		public override void HideFeedback() {
			Coffin.layer = _backupLayer;
		}
	}

	public class CarveTerrainInteraction : Interaction {

		private static GameObject _digMarker;

		private readonly Transform _transform;
		private readonly Transform _cameraTransform;
		private readonly CarveTerrainVolumeComponent _terrainCarver;
		private RaycastHit _hit;

		public CarveTerrainInteraction(RaycastHit hit) {
			Debug.Log("CONSTRUCT CARVE INTERACTION");
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

			_hit = hit;
			Player player = Player.GetInstance();
			_transform = player.transform;
			_cameraTransform = player.Camera.transform;

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

		public override void DoInteraction() {
			Debug.Log("DO DIG INTERACTION");

			Vector3[] v = _terrainCarver.DoCarveAction(new Ray(_transform.position, _cameraTransform.forward));
			GameObject tomb = new GameObject("_tomb");
			CreateTombComponent tombComponent = tomb.AddComponent<CreateTombComponent>();

			Vector3 continuousPosition = _hit.point - _hit.normal * 0.5f;
			Vector3 discretePosition = new Vector3((int)continuousPosition.x, (int)continuousPosition.y, (int)continuousPosition.z);
			tomb.transform.position = discretePosition; 
			tomb.transform.up = _hit.normal;

			Debug.DrawRay(v[0], Vector3.up*10, Color.magenta);
			Debug.DrawRay(v[1], Vector3.up*10, Color.magenta);
			Vector3 upperLeft = v[0];
			Vector3 lowerRight = v[1];
			Vector3 middleLow = new Vector3(upperLeft.x - 4.0f, _hit.point.y, lowerRight.z - (lowerRight.z - upperLeft.z)/2.0f );
			Player.GetInstance().MoveImmediatlyTo(middleLow);
			Player.GetInstance().transform.rotation =  Quaternion.AngleAxis(80, Vector3.up);
			Player.GetInstance().CurrentState = new DigDownState(tombComponent.GetGround());

		}

		public override void ShowFeedback() {
			_digMarker.SetActive(true);
			_digMarker.transform.position = _hit.point;
			_digMarker.transform.up = Vector3.Lerp(_digMarker.transform.up, _hit.normal, 0.05f);
		}

		public override void HideFeedback() {
			_digMarker.SetActive(false);
		}

		public void RefreshHit(RaycastHit hit) {
			_hit = hit;
		}
	}

	
	/*
	public class DriveCartInteraction : Interaction {
		
	}

	*/
}
