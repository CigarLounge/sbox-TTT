using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class HomePage : Panel
{
	private Button ForceSpectatorButton { get; init; }
	private Button RockTheVoteButton { get; init; }

	private bool _isRecordingModeEnabled = false;

	public void GoToKeyBindingsPage()
	{
		SettingsMenu.Instance.AddPage( new KeyBindingsPage() );
	}

	public void GoToCrosshairPage()
	{
		SettingsMenu.Instance.AddPage( new CrosshairPage() );
	}

	public void GoToComponentTesting()
	{
		SettingsMenu.Instance.AddPage( new ComponentTestingPage() );
	}

	public void ToggleRecordingMode()
	{
		_isRecordingModeEnabled = !_isRecordingModeEnabled;
		foreach ( var child in Local.Hud.Children )
		{
			if ( child is TabMenu )
				continue;

			child.Enabled( !_isRecordingModeEnabled );
		}
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		ForceSpectatorButton.Text = $"Force Spectator Mode ({(player.IsForcedSpectator ? "Enabled" : "Disabled")})";

		if ( Local.Client.GetValue<bool>( RawStrings.HasRockedTheVote ) )
			RockTheVoteButton.Text = "You have already rocked the vote!";
	}

	public void RockTheVote()
	{
		if ( Local.Client.GetValue<bool>( RawStrings.HasRockedTheVote ) )
			return;

		Game.RockTheVote();
	}

	public void ToggleForceSpectator()
	{
		Game.ToggleForceSpectator();
	}
}
