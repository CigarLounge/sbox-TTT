using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTT;

public class PreRound : BaseRound
{
	public override string RoundName => "Preparing";
	public override int RoundDuration => Game.PreRoundTime;

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		player.Respawn();
	}

	public override void OnPlayerKilled( Player player )
	{
		StartRespawnTimer( player );

		player.MakeSpectator();

		base.OnPlayerKilled( player );
	}

	protected override void OnStart()
	{
		MapHandler.CleanUp();

		if ( !Host.IsServer )
			return;

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player )
			{
				player.RemoveLogicButtons();
				player.Respawn();
			}
		}
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		List<Player> players = new();
		List<Player> spectators = new();

		foreach ( Player player in Utils.GetPlayers() )
		{
			player.Client.SetValue( RawStrings.Spectator, player.IsForcedSpectator );

			if ( player.IsForcedSpectator )
			{
				player.MakeSpectator( false );
				spectators.Add( player );
			}
			else
			{
				players.Add( player );
			}
		}

		AssignRolesAndRespawn( players );

		Game.Current.ChangeRound( new InProgressRound
		{
			Players = players,
			Spectators = spectators
		} );
	}

	private static void AssignRolesAndRespawn( List<Player> players )
	{
		int traitorCount = (int)Math.Max( players.Count * 0.25f, 1f );

		for ( int i = 0; i < traitorCount; i++ )
		{
			List<Player> unassignedPlayers = players.Where( p => p.Role is NoneRole ).ToList();
			int randomId = Rand.Int( 0, unassignedPlayers.Count - 1 );

			if ( unassignedPlayers[randomId].Role is NoneRole )
			{
				unassignedPlayers[randomId].SetRole( new TraitorRole() );
			}
		}

		int detectiveCount = (int)(players.Count * 0.125f);

		for ( int i = 0; i < detectiveCount; i++ )
		{
			List<Player> unassignedPlayers = players.Where( p => p.Role is NoneRole ).ToList();
			int randomId = Rand.Int( 0, unassignedPlayers.Count - 1 );

			if ( unassignedPlayers[randomId].Role is NoneRole )
			{
				unassignedPlayers[randomId].SetRole( new DetectiveRole() );
			}
		}

		foreach ( Player player in players )
		{
			if ( player.Role is NoneRole )
			{
				player.SetRole( new InnocentRole() );
			}

			using ( Prediction.Off() )
			{
				player.SendClientRole();
			}

			if ( player.LifeState == LifeState.Dead )
			{
				player.Respawn();
			}
			else
			{
				player.SetHealth( player.MaxHealth );
			}
		}
	}

	private static async void StartRespawnTimer( Player player )
	{
		try
		{
			await GameTask.DelaySeconds( 1 );

			if ( player.IsValid() && Game.Current.Round is PreRound )
			{
				player.Respawn();
			}
		}
		catch ( Exception e )
		{
			if ( e.Message.Trim() == "A task was canceled." )
			{
				return;
			}

			Log.Error( $"[TASK] {e.Message}: {e.StackTrace}" );
		}
	}

	public override void OnPlayerSpawn( Player player )
	{
		player.Inventory.Add( new Hands() );
		base.OnPlayerSpawn( player );
	}
}
