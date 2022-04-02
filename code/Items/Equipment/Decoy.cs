using Sandbox;
using System;

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
			var decoy = Owner.Inventory.DropEntity<DecoyEntity>( this );
		}
		else if ( Input.Pressed( InputButton.Attack2 ) )
		{
			var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
					.WorldOnly()
					.Run();

			if ( !trace.Hit )
				return;

			var decoy = Owner.Inventory.DropEntity<DecoyEntity>( this );
			decoy.Velocity = 0;
			decoy.Position = trace.EndPosition;
			decoy.Rotation = Rotation.From( trace.Normal.EulerAngles );
			decoy.MoveType = MoveType.None;

			if ( Math.Abs( trace.Normal.z ) >= 0.99f )
			{
				decoy.Rotation = Rotation.From
				(
					Rotation.Angles()
					.WithYaw( PreviousOwner.EyeRotation.Yaw() )
					.WithPitch( -90 * trace.Normal.z.CeilToInt() )
					.WithRoll( 180f )
				);
			}
		}
	}
}
