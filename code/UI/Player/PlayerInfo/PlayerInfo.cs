using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class PlayerInfo : Panel
{
	private Panel HealthContainer { get; set; }

	protected override int BuildHash() => HashCode.Combine( PlayerCamera.Target.Role, PlayerCamera.Target.Health );

	[GameEvent.Player.TookDamage]
	private void OnHit( Player _ )
	{
		if ( !this.IsEnabled() )
			return;

		HealthContainer.AddClass( "hit" );
		Utils.DelayAction( 0.2f, () => { HealthContainer.RemoveClass( "hit" ); } );
	}
}
