using Sandbox;

using TTT.Events;
using TTT.Globals;
using TTT.Items;
using TTT.UI;

namespace TTT.Player;

public partial class TTTPlayer
{
	[ClientRpc]
	public void ClientAnotherPlayerDidDamage( Vector3 position, float inverseHealth )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + inverseHealth * 1 )
			.SetPosition( position );
	}

	[ClientRpc]
	public void ClientTookDamage( Vector3 position, float damage )
	{
		DamageIndicator.Instance?.OnHit( position );
	}
}
