using Sandbox;

namespace TTT;

public partial class Player
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
		UI.DamageIndicator.Instance?.OnHit( position );
	}
}
