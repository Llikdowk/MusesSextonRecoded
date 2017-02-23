using Audio;
using UnityEngine;
using Cubiquity;
using UI;
using UnityEngine.UI;

public class LoadComponents : MonoBehaviour {
	public TerrainVolume Terrain;
	public Image TitleImage;
	public Image FallbackImage;
	public Camera UICamera;

	private float _originalTimeScale;
	private bool _hasClicked = false;

	public void Awake() {
		Terrain.gameObject.SetActive(true);
		Terrain.enabled = true;
		_originalTimeScale = Time.timeScale;
		Time.timeScale = 0.0f;
	}

	public void Start() {
		TitleImage.gameObject.SetActive(true);
		TitleImage.enabled = true;
		FallbackImage.gameObject.SetActive(true);
		FallbackImage.enabled = true;
	}

	public void Update() {
		if (Terrain.isMeshSyncronized && _hasClicked) {
			string terrainTag = Terrain.tag;
			foreach (Transform child in Terrain.GetComponentsInChildren<Transform>()) {
				child.tag = terrainTag;
			}
			enabled = false;

			UIGame.GetInstance().FadeOut(FallbackImage, 0.5f, () => {
				FallbackImage.gameObject.SetActive(false);
				Time.timeScale = _originalTimeScale;
			});

		}
		else {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				_hasClicked = true;

				UIGame.GetInstance().FadeOut(TitleImage, 3.0f, () => {
					TitleImage.gameObject.SetActive(false);
				});
				FallbackImage.gameObject.SetActive(true);

				AudioController.GetInstance().PlayBell();
				AudioController.GetInstance().FadeInWind();
			}
		}
	}
}
