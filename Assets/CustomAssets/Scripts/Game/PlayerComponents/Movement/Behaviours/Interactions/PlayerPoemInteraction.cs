using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using Game.Poems;
using UnityEngine;

public class PlayerPoemInteraction : Interaction {
	private readonly VersesDisplayer _displayMeshText = VersesDisplayer.PlayerPoem;
	private const float _distanceFromPlayer = 10.0f;
	private bool _hasHit = false;
	private GameObject _selectedGameObject;
	private VerseInfo _selectedVerse;

	public PlayerPoemInteraction() {
		DisplayVerses();
	}

	public void DisplayVerses() {
		string[] verses = Player.GetInstance().GetNextTombPoem();
		if (verses == null) {
			Player.GetInstance().CurrentState = new FinalGameState();
			return;
		}

		VerseInfo[] versesText = new VerseInfo[verses.Length];
		for (int i = 0; i < versesText.Length; ++i) {
			versesText[i].Verse = verses[i];
		}
		Player player = Player.GetInstance();
		_displayMeshText.Display(player.transform.position + player.transform.forward * _distanceFromPlayer, player.transform.rotation, versesText); // TODO parametrice distance
	}

	public override void DoInteraction() {
		if (_hasHit) {
			DisplayVerses();
			// TODO raiseTomb
		}
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
			_selectedGameObject = null;
		}
		return this;
	}
}