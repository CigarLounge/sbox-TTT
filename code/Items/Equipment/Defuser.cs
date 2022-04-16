using Sandbox;

namespace TTT;

[Library( "ttt_equipment_defuser", Title = "Defuser" )]
public partial class Defuser : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !Input.Pressed( InputButton.Attack1 ) )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( this )
			.Ignore( Owner )
			.EntitiesOnly()
			.Run();

		if ( trace.Entity is not C4Entity c4 )
			return;

		if ( c4.IsArmed )
			c4.Defuse();
	}
}
