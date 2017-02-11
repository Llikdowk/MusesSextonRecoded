using Audio;
using UnityEngine;
using Cubiquity;
using UnityEditor.Animations;
using UnityEngine.UI;
using Utils;

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
		AudioController.GetInstance().FadeInWind();
	}

	public void Update() {
		if (Terrain.isMeshSyncronized && _hasClicked) {
			string terrainTag = Terrain.tag;
			foreach (Transform child in Terrain.GetComponentsInChildren<Transform>()) {
				child.tag = terrainTag;
			}
			enabled = false;
			StartCoroutine(UI.FadeOut(FallbackImage, 0.5f, () => {
				FallbackImage.gameObject.SetActive(false);
				Time.timeScale = _originalTimeScale;
			}));
			AudioController.GetInstance().FadeInMusic1(
				()=>AudioController.GetInstance().FadeInMusic2(
					() => AudioController.GetInstance().FadeInMusic3(
						() => AudioController.GetInstance().FadeInPercussion()
					)
				)
			);
		}
		else {
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				_hasClicked = true;
				StartCoroutine(UI.FadeOut(TitleImage, 3.0f, () => {
					TitleImage.gameObject.SetActive(false);
				}));
				FallbackImage.gameObject.SetActive(true);
				AudioController.GetInstance().PlayBell();
			}
		}
	}
}
