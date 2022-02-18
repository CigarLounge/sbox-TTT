using Sandbox;

using TTT.Globals;
using TTT.Player;
using TTT.Rounds;
using TTT.Roles;

namespace TTT.Map;

[Library( "ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator." )]
public partial class LogicAssigned : Entity
{
	[Property( "Check Value", "Note that teams are often plural. For example, check the `Role` for `Traitor`, but check the `Team` for `Traitors`." )]
	public Role CheckRole { get; set; }

	/// <summary>
	/// Fires if activator's check type matches the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnPass { get; set; }

	/// <summary>
	/// Fires if activator's check type does not match the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
	/// </summary>
	protected Output OnFail { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Parent = Game.Current;
	}

	[Input]
	public void Activate( Entity activator )
	{
		if ( activator is TTTPlayer player && Gamemode.Game.Current.Round is InProgressRound )
		{
			if ( player.Role.ID == CheckRole )
			{
				OnPass.Fire( this );

				return;
			}

			OnFail.Fire( this );
		}
		else
		{
			Log.Warning( "ttt_logic_assigned: Activator is not player." );
			OnFail.Fire( this );
		}
	}
}
