using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_decoy", Title = "Decoy" )]
public partial class Decoy : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
			Owner.Inventory.DropEntity( this, typeof( DecoyEntity ) );
	}
}
