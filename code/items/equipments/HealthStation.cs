using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
public partial class HealthStation : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			Owner.Inventory.DropEntity( this, typeof( HealthStationEntity ) );
		}
	}
}
