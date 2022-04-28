using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class HomePage : Panel
{
	private Button ForceSpectatorButton { get; init; }
	private Button RockTheVoteButton { get; init; }

	private bool _isRecordingModeEnabled = false;

	public void GoToRoundSummaryPage()
	{
		Menu.Instance.AddPage( new RoundSummaryPage() );
	}

	public void GoToKeyBindingsPage()
	{
		Menu.Instance.AddPage( new KeyBindingsPage() );
	}

	public void GoToCrosshairPage()
	{
		Menu.Instance.AddPage( new CrosshairPage() );
	}

	public void GoToComponentTesting()
	{
		Menu.Instance.AddPage( new ComponentTestingPage() );
	}

	public void ToggleRecordingMode()
	{
		_isRecordingModeEnabled = !_isRecordingModeEnabled;
		foreach ( var child in Local.Hud.Children )
		{
			if ( child is TabController )
				continue;

			child.Enabled( !_isRecordingModeEnabled );
		}
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
