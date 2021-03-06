﻿using Audio;
using Game.Entities;
using Game.Poems;
using UnityEngine;

namespace Game.PlayerComponents.Movement.Behaviours.Interactions {

	public class VerseSelectionInteraction : Interaction {
		private readonly VersesDisplayer _displayMeshText = VersesDisplayer.NormalPoem;
		private LandmarkVerses _verses;
		protected VerseInfo _selectedVerse;
		protected GameObject _selectedGameObject;
		protected bool _hasHit;
		private readonly TombComponent _tombComponent;
		private const float _distanceFromPlayer = 10.0f;
		private readonly PoemState _poemState;

		public VerseSelectionInteraction(LandmarkVerses verses, PoemState.GenderEnum gender, TombComponent tombComponent) {
			_verses = verses;
			_tombComponent = tombComponent;
			_poemState = ((PoemState)Player.GetInstance().CurrentState); // TODO: send in constructor
			DisplayVerses(gender);
		}


		private void DisplayVerses(PoemState.GenderEnum gender) {
			VerseInfo[] versesText = new VerseInfo[6];

			switch (gender) {
				case PoemState.GenderEnum.Undefined:
					versesText[0].Verse = _verses.Masculine[0];
					versesText[0].Gender = PoemState.GenderEnum.Masculine;
					versesText[0].FirstPersonVerse = _verses.FirstPerson[0];
					versesText[1].Verse = _verses.Masculine[1];
					versesText[1].Gender = PoemState.GenderEnum.Masculine;
					versesText[1].FirstPersonVerse = _verses.FirstPerson[1];

					versesText[2].Verse = _verses.Feminine[2];
					versesText[2].Gender = PoemState.GenderEnum.Feminine;
					versesText[2].FirstPersonVerse = _verses.FirstPerson[2];
					versesText[3].Verse = _verses.Feminine[3];
					versesText[3].Gender = PoemState.GenderEnum.Feminine;
					versesText[3].FirstPersonVerse = _verses.FirstPerson[3];

					versesText[4].Verse = _verses.Plural[4];
					versesText[4].Gender = PoemState.GenderEnum.Plural;
					versesText[4].FirstPersonVerse = _verses.FirstPerson[4];
					versesText[5].Verse = _verses.Plural[5];
					versesText[5].Gender = PoemState.GenderEnum.Plural;
					versesText[5].FirstPersonVerse = _verses.FirstPerson[5];
					break;

				case PoemState.GenderEnum.Masculine:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Masculine[i];
						versesText[i].Gender = PoemState.GenderEnum.Masculine;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;

				case PoemState.GenderEnum.Feminine:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Feminine[i];
						versesText[i].Gender = PoemState.GenderEnum.Feminine;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;

				case PoemState.GenderEnum.Plural:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Plural[i];
						versesText[i].Gender = PoemState.GenderEnum.Plural;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;
			}


			Player player = Player.GetInstance();
			_displayMeshText.Display(player.MainCamera.transform.position + player.MainCamera.transform.forward * _distanceFromPlayer,
				player.MainCamera.transform.rotation, versesText); // TODO parametrice distance
		}


		private static int VersesChosen = 0; // TODO find another way
		private const int MaxCount = 3;
		private bool _animationRunning = false;
		public override void DoInteraction() {
			if (_selectedVerse.Verse != null) {
				AudioController.GetInstance().PlayTone();
				if (_poemState.Gender == PoemState.GenderEnum.Undefined) {
					_poemState.Gender = _selectedVerse.Gender;
				}
				Player.GetInstance().AddPoemVerse(_selectedVerse.FirstPersonVerse);
				_tombComponent.AddVerse(_selectedVerse.Verse);
				_tombComponent.PlayerTombRefocus();


				Player.GetInstance().CameraController.DisableDepthOfField(0.25f);
				Player.GetInstance().CameraController.Unsaturate(0.0f, 1.0f);
				Player.GetInstance().ShowShovel();
				Player.GetInstance().PlayerShovelAnimationEnding = () => Player.GetInstance().HideShovel();
				if (Player.GetInstance().PlayDigAnimation()) {
					_animationRunning = true;
					_tombComponent.Bury(() => {
						_poemState.SetLandmarkSelectionInteraction();
						_animationRunning = false;
					});
					++VersesChosen;
					if (VersesChosen >= MaxCount) {
						Player.GetInstance().PlayerShovelAnimationEnding = () => {
							++GameState.CoffinsBuried;
							Player.GetInstance().CurrentState = new WalkRunState();
							_tombComponent.MarkForFinished();
							VersesChosen = 0;
						};
					}
				}
			}

			else {
				if (!_animationRunning) {
					_poemState.SetLandmarkSelectionInteraction();
				}
			}
			_displayMeshText.HideSmooth();
			_selectedGameObject = null;
			_selectedVerse.Verse = null;
		}

		public override void ShowFeedback() {
			if (_selectedGameObject != null) {
				_selectedGameObject.GetComponent<TextMesh>()
					.GetComponent<Renderer>().material.color = _displayMeshText.BaseHighlightColor;
			}
		}

		public override void HideFeedback() {
			if (_selectedGameObject != null) {
				_selectedGameObject.GetComponent<TextMesh>()
					.GetComponent<Renderer>().material.color = _displayMeshText.BaseColor;
			}
		}

		public override Interaction CheckForPromotion() {
			Ray ray = new Ray(Player.GetInstance().MainCamera.transform.position, Player.GetInstance().MainCamera.transform.forward);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.red);
			_hasHit = Physics.SphereCast(ray, 0.05f, out hit, 1000.0f, 1 << LayerMaskManager.Get(Layer.Verse),
				QueryTriggerInteraction.Collide);

			if (_hasHit) {
				if (_selectedGameObject == null) {
					_selectedGameObject = hit.collider.gameObject;
					_selectedVerse = _selectedGameObject.GetComponent<VerseInfoComponent>().Info;
				}
				if (_selectedGameObject.GetInstanceID() != hit.collider.gameObject.GetInstanceID()) {
					HideFeedback();
					_selectedGameObject = hit.collider.gameObject;
					_selectedVerse = _selectedGameObject.GetComponent<VerseInfoComponent>().Info;
					ShowFeedback();
				}
			}
			else {
				HideFeedback();
				_selectedGameObject = null;
				_selectedVerse.Verse = null;
			}
			return this;
		}
	}
}