using System;
using Sandbox;

using TTT.Player;
using TTT.Roles;

namespace TTT.Items;

[Hammer.Skip]
[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
public partial class HealthStation : Carriable
{
	public override void Simulate( Client client )
	{
		if ( Owner is not TTTPlayer owner || !IsServer )
			return;

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Attack1 ) )
			{
				owner.Inventory.DropEntity( this, typeof( HealthStationEntity ) );
			}
		}
	}

	public bool CanDrop() { return false; }
}
