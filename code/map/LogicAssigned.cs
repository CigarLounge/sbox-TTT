using Sandbox;

namespace TTT;

[Library( "ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator." )]
public partial class LogicAssigned : Entity
{
	[Property( "Check Value", "Note that teams are often plural. For example, check the `Role` for `Traitor`, but check the `Team` for `Traitors`." )]
	public Team CheckTeam { get; set; }

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
			if ( player.Role.Info.Team == CheckTeam )
			{
				_ = OnPass.Fire( this );
				return;
			}

			_ = OnFail.Fire( this );
		}
		else
		{
			Log.Warning( "ttt_logic_assigned: Activator is not player." );
			_ = OnFail.Fire( this );
		}
	}
}
