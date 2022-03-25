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
		{
			Owner.Inventory.DropEntity( this, new DecoyEntity() );
		}
		else if ( Input.Pressed( InputButton.Attack2 ) )
		{
			var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.INTERACT_DISTANCE )
					.WorldOnly()
					.Run();

			if ( !trace.Hit )
				return;

			var decoy = Owner.Inventory.DropEntity( this, new DecoyEntity() );
			decoy.Velocity = 0;
			decoy.Position = trace.EndPosition;
			decoy.Rotation = Rotation.From( trace.Normal.EulerAngles );
			decoy.MoveType = MoveType.None;
		}
	}
}
