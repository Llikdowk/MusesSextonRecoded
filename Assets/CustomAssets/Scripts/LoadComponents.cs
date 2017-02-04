using UnityEngine;
using Cubiquity;

public class LoadComponents : MonoBehaviour {
	public TerrainVolume Terrain;
	private float _originalTimeScale;

	public void Awake() {
		_originalTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
	}

	public void Update() {
		if (Terrain.isMeshSyncronized) {
			Time.timeScale = _originalTimeScale;
			enabled = false;
		}
	}
}
