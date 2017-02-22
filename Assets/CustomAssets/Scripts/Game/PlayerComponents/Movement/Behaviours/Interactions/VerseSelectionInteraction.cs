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

		public VerseSelectionInteraction(LandmarkVerses verses, PoemState.GenderEnum gender, TombComponent tombComponent) {
			_verses = verses;
			DisplayVerses(gender);
			_tombComponent = tombComponent;
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
			_displayMeshText.Display(player.transform.position + player.transform.forward * _distanceFromPlayer, player.transform.rotation, versesText); // TODO parametrice distance
		}


		private static int _counter = 0;
		private const int MaxCount = 3;
		public override void DoInteraction() {
			PoemState poemState = ((PoemState)Player.GetInstance().CurrentState); // TODO: send in constructor
			if (_hasHit) {
				if (poemState.Gender == PoemState.GenderEnum.Undefined) {
					poemState.Gender = _selectedVerse.Gender;
				}
				Player.GetInstance().AddPoemVerse(_selectedVerse.FirstPersonVerse);
				_tombComponent.AddVerse(_selectedVerse.Verse);
				_tombComponent.PlayerTombRefocus();


				Player.GetInstance().CameraController.DisableDepthOfField(0.25f);
				Player.GetInstance().CameraController.Unsaturate(0.0f, 1.0f);
				Player.GetInstance().ShowShovel();
				Player.GetInstance().AnimationEnding = () => Player.GetInstance().HideShovel();
				if (Player.GetInstance().PlayDigAnimation()) {
					_tombComponent.Bury(() => {
						poemState.SetLandmarkSelectionInteraction();
					});
					++_counter;
					if (_counter >= MaxCount) {
						Player.GetInstance().AnimationEnding = () => {
							++GameState.CoffinsBuried;
							Player.GetInstance().CurrentState = new WalkRunState();
							_tombComponent.MarkForFinished();
						};
						_counter = 0;
					}
				}


			}
			else {
				poemState.CalcNextInteraction();
			}
			_displayMeshText.Hide();
		}

		public override void ShowFeedback() {
			_selectedGameObject.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = _displayMeshText.BaseHighlightColor;
		}

		public override void HideFeedback() {
			_selectedGameObject.GetComponent<TextMesh>().GetComponent<Renderer>().material.color = _displayMeshText.BaseColor;
		}

		public override Interaction CheckForPromotion() {
			Ray ray = new Ray(Player.GetInstance().MainCamera.transform.position, Player.GetInstance().MainCamera.transform.forward);
			RaycastHit hit;
			Debug.DrawRay(ray.origin, ray.direction, Color.red);
			_hasHit = Physics.SphereCast(ray, 0.05f, out hit, 1000.0f, 1 << LayerMaskManager.Get(Layer.Verse),
				QueryTriggerInteraction.Collide);

			if (_hasHit) {
				if (_selectedGameObject == null) _selectedGameObject = hit.collider.gameObject;
				if (_selectedGameObject.GetInstanceID() != hit.collider.gameObject.GetInstanceID()) {
					HideFeedback();
					_selectedGameObject = hit.collider.gameObject;
					_selectedVerse = _selectedGameObject.GetComponent<VerseInfoComponent>().Info;
					ShowFeedback();
				}
			}
			else {
				HideFeedback();
			}
			return this;
		}
	}
}