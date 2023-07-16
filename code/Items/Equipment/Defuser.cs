using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_defuser" )]
[Title( "Defuser" )]
public class Defuser : Carriable
{
	public override void Simulate( IClient client )
	{
		if ( !Input.Pressed( InputAction.PrimaryAttack ) )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( this )
			.Ignore( Owner )
			.DynamicOnly()
			.Run();

		if ( trace.Entity is not C4Entity c4 )
			return;

		if ( c4.IsArmed )
			c4.Defuse();
	}
}
