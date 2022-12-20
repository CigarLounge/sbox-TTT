using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PlayerInfo : Panel
{
	private Panel HealthContainer { get; set; }

	protected override int BuildHash() => HashCode.Combine( PlayerCamera.Target.Role, PlayerCamera.Target.Health );

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
