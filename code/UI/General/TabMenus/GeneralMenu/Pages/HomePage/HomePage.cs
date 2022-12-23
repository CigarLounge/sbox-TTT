using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class HomePage : Panel
{
	public void GoToRoundSummaryPage()
	{
		GeneralMenu.Instance.AddPage( new RoundSummaryPage() );
	}

	public void GoToKeyBindingsPage()
	{
		GeneralMenu.Instance.AddPage( new KeyBindingsPage() );
	}

	public void RockTheVote()
	{
		GameManager.RockTheVote();
	}

	public void ToggleForceSpectator()
	{
		GameManager.ToggleForceSpectator();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( (Game.LocalPawn as Player).IsForcedSpectator, Game.LocalClient.GetValue<bool>( Strings.HasRockedTheVote ) );
	}
}
