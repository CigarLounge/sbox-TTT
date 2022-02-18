using Sandbox;
using TTT.Gamemode;
using TTT.Player;
using TTT.Settings;

namespace TTT.Rounds;

public class PostRound : BaseRound
{
	public override string RoundName => "Post";
	public override int RoundDuration { get => Gamemode.Game.PostRoundTime; }

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		RPCs.ClientClosePostRoundMenu();

		bool shouldChangeMap = Gamemode.Game.Current.MapSelection.TotalRoundsPlayed >= Gamemode.Game.RoundLimit;
		Gamemode.Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

	public override void OnPlayerKilled( TTTPlayer player )
	{
		player.MakeSpectator();
	}

	protected override void OnStart()
	{
		if ( Host.IsServer )
		{
			using ( Prediction.Off() )
			{
				foreach ( TTTPlayer player in Utils.GetPlayers() )
				{
					if ( player.PlayerCorpse != null && player.PlayerCorpse.IsValid() && player.LifeState == LifeState.Dead && !player.PlayerCorpse.IsIdentified )
					{
						player.PlayerCorpse.IsIdentified = true;

						RPCs.ClientConfirmPlayer( null, player.PlayerCorpse, player, player.PlayerCorpse.DeadPlayerClientData.Name, player.PlayerCorpse.DeadPlayerClientData.PlayerId, player.Role.Name, player.Team.Name, player.PlayerCorpse.GetConfirmationData(), player.PlayerCorpse.KillerWeapon, player.PlayerCorpse.Perks );
					}
					else
					{
						player.SendClientRole( To.Everyone );
					}
				}
			}
		}
	}
}
