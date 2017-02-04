using System.Collections.Generic;
using UnityEngine;
using Cubiquity;
using Game;
using UnityEngine.UI;

public class LoadComponents : MonoBehaviour {
	public TerrainVolume Terrain;
	public Image TitleImage;
	public Image FallbackImage;
	public Camera UICamera;

	private float _originalTimeScale;
	private readonly ActionManager _actions = ActionManager.GetInstance();
	private readonly List<GameObject> _startEnabledComponents = new List<GameObject>();

	public void Awake() {
		_originalTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
		_actions.AllowActions = false;
	}

	public void Start() {
		TitleImage.gameObject.SetActive(true);
		TitleImage.enabled = true;
		if (FallbackImage) {
			FallbackImage.gameObject.SetActive(true);
			FallbackImage.enabled = true;
		}
	}

	public void Update() {
		if (Terrain.isMeshSyncronized) {
			Time.timeScale = _originalTimeScale;
			enabled = false;
			TitleImage.gameObject.SetActive(false);
			if (FallbackImage) {
				FallbackImage.gameObject.SetActive(false);
			}
			_actions.AllowActions = true;
		}
		else {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				TitleImage.gameObject.SetActive(false);
				if (FallbackImage) {
					FallbackImage.gameObject.SetActive(true);
				}
			}
		}
	}
}
