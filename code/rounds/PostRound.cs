using Sandbox;

namespace TTT;

public class PostRound : BaseRound
{
	public override string RoundName => "Post";
	public override int RoundDuration => Game.PostRoundTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		RPCs.ClientClosePostRoundMenu();

		bool shouldChangeMap = Game.Current.MapSelection.TotalRoundsPlayed >= Game.RoundLimit;
		Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

	public override void OnPlayerKilled( Player player )
	{
		player.MakeSpectator();
	}

	protected override void OnStart()
	{
		if ( Host.IsServer )
		{
			using ( Prediction.Off() )
			{
				foreach ( Player player in Utils.GetPlayers() )
				{
					if ( player.PlayerCorpse != null && player.PlayerCorpse.IsValid() && player.LifeState == LifeState.Dead && !player.PlayerCorpse.IsIdentified )
					{
						player.PlayerCorpse.IsIdentified = true;

						RPCs.ClientConfirmPlayer( null, player.PlayerCorpse, player, player.PlayerCorpse.DeadPlayerClientData.Name, player.PlayerCorpse.DeadPlayerClientData.PlayerId, player.Role.ClassInfo.Name, player.PlayerCorpse.GetConfirmationData(), player.PlayerCorpse.KillerWeapon.LibraryName, player.PlayerCorpse.Perks );
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
