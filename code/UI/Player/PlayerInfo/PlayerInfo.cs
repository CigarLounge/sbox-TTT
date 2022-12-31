using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PlayerInfo : Panel
{
	private Panel HealthContainer { get; set; }

	protected override int BuildHash()
	{
		var player = Spectating.Player ?? (Player)Game.LocalPawn;

		return HashCode.Combine( player.Role, player.Health );
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
