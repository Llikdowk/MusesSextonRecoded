using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using Game.Poems;

public class PlayerPoemInteraction : Interaction {
	private readonly VersesDisplayer _displayMeshText = VersesDisplayer.PlayerPoem;
	private const float _distanceFromPlayer = 10.0f;

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
		/*
		if (_hasHit) {
			// TODO RAISE TOMB
		}
		*/
		_displayMeshText.Hide();
		DisplayVerses();
	}


	public override Interaction CheckForPromotion() {
		return this;
	}
}