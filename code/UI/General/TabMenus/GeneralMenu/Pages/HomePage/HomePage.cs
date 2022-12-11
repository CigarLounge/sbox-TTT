using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class HomePage : Panel
{
	private Button ForceSpectatorButton { get; init; }
	private Button RockTheVoteButton { get; init; }

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

		var player = Game.LocalPawn as Player;

		ForceSpectatorButton.Text = $"Force Spectator Mode - {(player.IsForcedSpectator ? "Enabled" : "Disabled")}";
		RockTheVoteButton.SetClass( "inactive", Game.LocalClient.GetValue<bool>( Strings.HasRockedTheVote ) );
	}

	public void RockTheVote()
	{
		TTTGame.RockTheVote();
	}

	public void ToggleForceSpectator()
	{
		TTTGame.ToggleForceSpectator();
	}
}
