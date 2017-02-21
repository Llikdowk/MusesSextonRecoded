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


		public VerseSelectionInteraction(LandmarkVerses verses, PoemState.Gender gender, TombComponent tombComponent) {
			_verses = verses;
			DisplayVerses(gender);
			_tombComponent = tombComponent;
		}


		private void DisplayVerses(PoemState.Gender gender) {
			VerseInfo[] versesText = new VerseInfo[6];

			switch (gender) {
				case PoemState.Gender.Undefined:
					versesText[0].Verse = _verses.Masculine[0];
					versesText[0].Gender = PoemState.Gender.Masculine;
					versesText[0].FirstPersonVerse = _verses.FirstPerson[0];
					versesText[1].Verse = _verses.Masculine[1];
					versesText[1].Gender = PoemState.Gender.Masculine;
					versesText[1].FirstPersonVerse = _verses.FirstPerson[1];

					versesText[2].Verse = _verses.Feminine[2];
					versesText[2].Gender = PoemState.Gender.Feminine;
					versesText[2].FirstPersonVerse = _verses.FirstPerson[2];
					versesText[3].Verse = _verses.Feminine[3];
					versesText[3].Gender = PoemState.Gender.Feminine;
					versesText[3].FirstPersonVerse = _verses.FirstPerson[3];

					versesText[4].Verse = _verses.Plural[4];
					versesText[4].Gender = PoemState.Gender.Plural;
					versesText[4].FirstPersonVerse = _verses.FirstPerson[4];
					versesText[5].Verse = _verses.Plural[5];
					versesText[5].Gender = PoemState.Gender.Plural;
					versesText[5].FirstPersonVerse = _verses.FirstPerson[5];
					break;

				case PoemState.Gender.Masculine:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Masculine[i];
						versesText[i].Gender = PoemState.Gender.Masculine;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;

				case PoemState.Gender.Feminine:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Feminine[i];
						versesText[i].Gender = PoemState.Gender.Feminine;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;

				case PoemState.Gender.Plural:
					for (int i = 0; i < versesText.Length; ++i) {
						versesText[i].Verse = _verses.Plural[i];
						versesText[i].Gender = PoemState.Gender.Plural;
						versesText[i].FirstPersonVerse = _verses.FirstPerson[i];
					}
					break;
			}


			Player player = Player.GetInstance();
			_displayMeshText.Display(player.transform.position + player.transform.forward * 6.0f, player.transform.rotation, versesText); // TODO parametrice distance
		}


		public override void DoInteraction() {
			PoemState poemState = ((PoemState)Player.GetInstance().CurrentState); // TODO: send in constructor
			if (_hasHit) {
				poemState.SetGender(_selectedVerse.Gender);
				Player.GetInstance().AddPoemVerse(_selectedVerse.FirstPersonVerse);
				_tombComponent.PlayerTombRefocus(new BuryState(_tombComponent));
				//Player.GetInstance().CurrentState = new BuryState(_tombComponent);
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
			Ray ray = new Ray(Player.GetInstance().Camera.transform.position, Player.GetInstance().Camera.transform.forward);
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