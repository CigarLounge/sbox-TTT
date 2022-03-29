using Sandbox;

namespace TTT;

[Hammer.EntityTool( "Role Check", "TTT", "Used to test the assigned role of the activator." )]
[Library( "ttt_role_check" )]
public partial class RoleCheck : Entity
{
	[Property( "Check Value", "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	public string Role { get; set; }

	/// <summary>
	/// Fires if activator's check type matches the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnPass { get; set; }

	/// <summary>
	/// Fires if activator's check type does not match the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnFail { get; set; }

	[Input]
	public void Activate( Entity activator )
	{
		if ( activator is Player player && Game.Current.Round is InProgressRound )
		{
			if ( player.Role == Role )
			{
				_ = OnPass.Fire( this );
				return;
			}

			_ = OnFail.Fire( this );
		}
		else
		{
			_ = OnFail.Fire( this );
		}
	}
}
