using Game;
using Game.PlayerComponents;
using Game.PlayerComponents.Movement.Behaviours.Interactions;
using Game.Poems;

public class PlayerPoemInteraction : VerseSelectionInteraction {
	private readonly VersesDisplayer _displayMeshText = VersesDisplayer.PlayerPoem;

	public PlayerPoemInteraction() : base(default(LandmarkVerses), PoemState.GenderEnum.FirstPerson, null) {
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
		_displayMeshText.Display(player.transform.position + player.transform.forward * 6.0f, player.transform.rotation, versesText); // TODO parametrice distance
	}

	public override void DoInteraction() {
		if (_hasHit) {
			// TODO RAISE TOMB
		}
		_displayMeshText.Hide();
		DisplayVerses();
	}
}