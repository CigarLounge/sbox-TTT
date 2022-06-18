using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_role_check" )]
[Description( "Checks for the specified role." )]
[HammerEntity]
[Title( "Role Check" )]
public partial class RoleCheck : Entity
{
	[Description( "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	[Title( "Check Value" )]
	[Property]
	public string Role { get; set; } = "Traitor";

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
		if ( activator is Player player && Game.Current.State is InProgress )
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