using Sandbox;

namespace TTT.Items;

[Hammer.Skip]
[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
public class HealthStation : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( !Input.Pressed( InputButton.Attack1 ) )
			return;

		Owner.Inventory.DropEntity<Entities.HealthStation>( this );
	}
}
