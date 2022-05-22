using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class HomePage : Panel
{
	private Button RoundSummaryButton { get; init; }
	private Button ForceSpectatorButton { get; init; }
	private Button RockTheVoteButton { get; init; }

	private bool _isRecordingModeEnabled = false;

	public void GoToRoundSummaryPage()
	{
		GeneralMenu.Instance.AddPage( new RoundSummaryPage() );
	}

	public void GoToKeyBindingsPage()
	{
		GeneralMenu.Instance.AddPage( new KeyBindingsPage() );
	}

	public void GoToCrosshairPage()
	{
		GeneralMenu.Instance.AddPage( new CrosshairPage() );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;

		ForceSpectatorButton.Text = $"Force Spectator Mode - {(player.IsForcedSpectator ? "Enabled" : "Disabled")}";
		RockTheVoteButton.SetClass( "inactive", Local.Client.GetValue<bool>( Strings.HasRockedTheVote ) );
	}

	public void RockTheVote()
	{
		Game.RockTheVote();
	}

	public void ToggleForceSpectator()
	{
		Game.ToggleForceSpectator();
	}
}
