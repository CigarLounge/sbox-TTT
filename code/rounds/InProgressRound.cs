using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTT.Events;
using TTT.Items;
using TTT.Map;
using TTT.Player;
using TTT.Settings;
using TTT.Teams;

namespace TTT.Rounds
{
	public partial class InProgressRound : BaseRound
	{
		public override string RoundName => "In Progress";

		[Net]
		public List<TTTPlayer> Players { get; set; }

		[Net]
		public List<TTTPlayer> Spectators { get; set; }

		private List<TTTLogicButton> _logicButtons;

		public override int RoundDuration
		{
			get => ServerSettings.Instance.Round.RoundTime;
		}

		public override void OnPlayerKilled( TTTPlayer player )
		{
			Players.Remove( player );
			Spectators.AddIfDoesNotContain( player );

			player.MakeSpectator();
			ChangeRoundIfOver();
		}

		public override void OnPlayerJoin( TTTPlayer player )
		{
			Spectators.AddIfDoesNotContain( player );
		}

		public override void OnPlayerLeave( TTTPlayer player )
		{
			Players.Remove( player );
			Spectators.Remove( player );

			ChangeRoundIfOver();
		}

		protected override void OnStart()
		{
			if ( Host.IsServer )
			{
				// For now, if the RandomWeaponCount of the map is zero, let's just give the players
				// a fixed weapon loadout.
				if ( Gamemode.Game.Instance.MapHandler.RandomWeaponCount == 0 )
				{
					foreach ( TTTPlayer player in Players )
					{
						GiveFixedLoadout( player );
					}
				}

				// Cache buttons for OnSecond tick.
				_logicButtons = Entity.All.Where( x => x.GetType() == typeof( TTTLogicButton ) ).Select( x => x as TTTLogicButton ).ToList();
			}
		}

		private static void GiveFixedLoadout( TTTPlayer player )
		{
			Log.Debug( $"Added Fixed Loadout to {player.Client.Name}" );

			player.GiveAmmo( SWB_Base.AmmoType.SMG, 9000 );
		}

		protected override void OnTimeUp()
		{
			LoadPostRound( TeamFunctions.GetTeam( typeof( InnocentTeam ) ) );

			base.OnTimeUp();
		}

		private TTTTeam IsRoundOver()
		{
			List<TTTTeam> aliveTeams = new();

			foreach ( TTTPlayer player in Players )
			{
				if ( player.Team != null && !aliveTeams.Contains( player.Team ) )
				{
					aliveTeams.Add( player.Team );
				}
			}

			if ( aliveTeams.Count == 0 )
			{
				return TeamFunctions.GetTeam( typeof( NoneTeam ) );
			}

			return aliveTeams.Count == 1 ? aliveTeams[0] : null;
		}

		public static void LoadPostRound( TTTTeam winningTeam )
		{
			Gamemode.Game.Instance.MapSelection.TotalRoundsPlayed++;
			Gamemode.Game.Instance.ForceRoundChange( new PostRound() );
			RPCs.ClientOpenAndSetPostRoundMenu(
				winningTeam.Name,
				winningTeam.Color
			);
		}

		public override void OnSecond()
		{
			if ( Host.IsServer )
			{
				if ( !ServerSettings.Instance.Debug.PreventWin )
				{
					base.OnSecond();
				}
				else
				{
					RoundEndTime += 1f;
				}

				_logicButtons.ForEach( x => x.OnSecond() ); // Tick role button delay timer.

				if ( !Utils.HasMinimumPlayers() && IsRoundOver() == null )
				{
					Gamemode.Game.Instance.ForceRoundChange( new WaitingRound() );
				}
			}
		}

		private bool ChangeRoundIfOver()
		{
			TTTTeam result = IsRoundOver();

			if ( result != null && !Settings.ServerSettings.Instance.Debug.PreventWin )
			{
				LoadPostRound( result );

				return true;
			}

			return false;
		}

		[Event( TTTEvent.Player.Role.Select )]
		private static void OnPlayerRoleChange( TTTPlayer player )
		{
			if ( Host.IsClient )
			{
				return;
			}

			if ( Gamemode.Game.Instance.Round is InProgressRound inProgressRound )
			{
				inProgressRound.ChangeRoundIfOver();
			}
		}

		[Event( TTTEvent.Settings.Change )]
		private static void OnChangeSettings()
		{
			if ( Gamemode.Game.Instance.Round is not InProgressRound inProgressRound )
			{
				return;
			}

			inProgressRound.ChangeRoundIfOver();
		}
	}
}
