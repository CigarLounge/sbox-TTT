using Sandbox;
using System;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_c4", Title = "C4" )]
public partial class C4 : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			Owner.Inventory.DropEntity<C4Entity>( this );
		}
		else if ( Input.Pressed( InputButton.Attack2 ) )
		{
			var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
					.WorldOnly()
					.Run();

			if ( !trace.Hit )
				return;

			var c4 = Owner.Inventory.DropEntity<C4Entity>( this );
			c4.Velocity = 0;
			c4.Position = trace.EndPosition;
			c4.Rotation = Rotation.From( trace.Normal.EulerAngles );
			c4.MoveType = MoveType.None;

			if ( Math.Abs( trace.Normal.z ) >= 0.99f )
			{
				c4.Rotation = Rotation.From
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
