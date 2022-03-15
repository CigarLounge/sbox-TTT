using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_radio", Title = "Radio" )]
public partial class Radio : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
			Owner.Inventory.DropEntity( this, typeof( RadioEntity ) );
	}
}
