using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PlayerInfo : Panel
{
	private Panel HealthContainer { get; set; }

	protected override int BuildHash()
	{
		return HashCode.Combine( Hud.DisplayedPlayer.SteamId, Hud.DisplayedPlayer.Role, Hud.DisplayedPlayer.Health );
	}

	[GameEvent.Player.TookDamage]
	private async void OnHit( Player _ )
	{
		if ( !this.IsEnabled() )
			return;

		AddClass( "hit" );
		await GameTask.Delay( 200 );
		RemoveClass( "hit" );
	}
}
