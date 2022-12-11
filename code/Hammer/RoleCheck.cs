using Editor;
using Sandbox;

namespace TTT;

[Category( "Hammer Logic" )]
[ClassName( "ttt_role_check" )]
[Description( "Checks for the specified role." )]
[HammerEntity]
[Title( "Role Check" )]
public partial class RoleCheck : Entity
{
	[Title( "Check Value" )]
	[Description( "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	[Property]
	public string Role { get; private set; } = "Traitor";

	/// <summary>
	/// Fires if activator's check type matches the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnPass { get; private set; }

	/// <summary>
	/// Fires if activator's check type does not match the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnFail { get; private set; }

	public override void Spawn()
	{
		Transmit = TransmitType.Never;
	}

	[Input]
	public void Activate( Entity activator )
	{
		if ( GameManager.Current.State is not InProgress )
			return;

		if ( activator is Player player )
		{
			if ( player.Role == Role )
			{
				_ = OnPass.Fire( this );
				return;
			}
		}

		_ = OnFail.Fire( this );
	}
}
