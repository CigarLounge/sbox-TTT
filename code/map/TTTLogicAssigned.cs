using Sandbox;

using TTT.Globals;
using TTT.Player;
using TTT.Rounds;
using TTT.Teams;

namespace TTT.Map
{
	[Library( "ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator." )]
	public partial class TTTLogicAssigned : Entity
	{
		[Property( "Check Value", "Note that teams are often plural. For example, check the `Role` for `Traitor`, but check the `Team` for `Traitors`." )]
		public string CheckValue
		{
			get => _checkValue;
			set
			{
				_checkValue = value?.ToLower();
			}
		}
		private string _checkValue = Utils.GetLibraryName( typeof( TraitorTeam ) );

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
			if ( activator is TTTPlayer player && Gamemode.Game.Instance.Round is InProgressRound )
			{
				if ( player.Role.Name.ToLower().Equals( CheckValue ) )
				{
					OnPass.Fire( this );

					return;
				}
				else if ( player.Team.Name.ToLower().Equals( CheckValue ) )
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
}
