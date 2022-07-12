using Sandbox;
using System.Collections.Generic;

namespace TTT;

public enum PlayerStatus
{
	Alive,
	MissingInAction,
	ConfirmedDead,
	Spectator
}

public partial class Player
{
	[Net]
	public long SteamId { get; set; }

	[Net]
	public string SteamName { get; set; }

	public Corpse Corpse { get; private set; }

	/// <summary>
	/// The player who confirmed this player's corpse.
	/// </summary>
	public Player Confirmer { get; private set; }
	public bool IsMissingInAction => Status == PlayerStatus.MissingInAction;
	public bool IsConfirmedDead => Status == PlayerStatus.ConfirmedDead;
	public bool IsRoleKnown { get; set; }
	public string LastSeenPlayerName { get; set; }
	public List<Player> PlayersKilled { get; set; } = new();

	private PlayerStatus _status;
	public PlayerStatus Status
	{
		get => _status;
		set
		{
			if ( _status == value )
				return;

			var oldStatus = _status;
			_status = value;

			Event.Run( TTTEvent.Player.StatusChanged, this, oldStatus );
		}
	}

	public void RemoveCorpse()
	{
		Host.AssertServer();

		if ( !Corpse.IsValid() )
			return;

		Corpse.Delete();
		Corpse = null;
	}

	private void BecomeCorpse()
	{
		Host.AssertServer();

		Corpse = new Corpse( this );
	}

	public void UpdateMissingInAction( Player player = null )
	{
		Host.AssertServer();

		if ( player is not null )
		{
			ClientSetStatus( To.Single( player ), PlayerStatus.MissingInAction );
			return;
		}

		Status = PlayerStatus.MissingInAction;
		ClientSetStatus( Team.Traitors.ToClients(), PlayerStatus.MissingInAction );

		if ( Team != Team.Traitors )
			ClientSetStatus( To.Single( this ), PlayerStatus.MissingInAction );
	}

	/// <summary>
	/// Confirm the player. The player will only be labeled 
	/// as <see cref="PlayerStatus.ConfirmedDead"/> if the target is <see cref="To.Everyone"/>.
	/// </summary>
	/// <param name="to">The target.</param>
	/// <param name="confirmer">
	/// The player who confirmed the corpse for the rest of the lobby.
	/// </param>
	public void Confirm( To to, bool global = false, Player confirmer = null )
	{
		Host.AssertServer();

		if ( this.IsAlive() || IsSpectator )
		{
			Log.Warning( "Trying to confirm an alive player or spectator!" );
			return;
		}

		var wasPreviouslyConfirmed = true;

		if ( !IsConfirmedDead && global )
		{
			Confirmer = confirmer;
			IsRoleKnown = true;
			Status = PlayerStatus.ConfirmedDead;
			ClientSetStatus( PlayerStatus.ConfirmedDead );
			wasPreviouslyConfirmed = false;
		}

		SendRole( to );
		ClientSetCorpse( to, Corpse );
		ClientConfirm( to, Confirmer, wasPreviouslyConfirmed );
	}

	private void CheckLastSeenPlayer()
	{
		if ( HoveredEntity is Player player && player.CanHint( this ) )
			LastSeenPlayerName = player.Client.Name;
	}

	private void ResetConfirmationData()
	{
		Confirmer = null;
		Corpse = null;
		IsRoleKnown = false;
		LastSeenPlayerName = string.Empty;
		PlayersKilled.Clear();
	}

	[ClientRpc]
	private void ClientConfirm( Player confirmer, bool wasPreviouslyConfirmed = false )
	{
		Confirmer = confirmer;
		Status = IsConfirmedDead ? PlayerStatus.ConfirmedDead : PlayerStatus.MissingInAction;

		if ( wasPreviouslyConfirmed || !Confirmer.IsValid() || !Corpse.IsValid() )
			return;

		Event.Run( TTTEvent.Player.CorpseFound, this );
	}

	[ClientRpc]
	private void ClientSetCorpse( Corpse corpse )
	{
		Corpse = corpse;
		Corpse.Player = this;
	}

	[ClientRpc]
	private void ClientSetStatus( PlayerStatus status )
	{
		Status = status;
	}

	[TTTEvent.Game.ClientJoined]
	private void SyncClient( Client client )
	{
		if ( IsSpectator )
			ClientSetStatus( To.Single( client ), PlayerStatus.Spectator );

		if ( IsConfirmedDead )
			Confirm( To.Single( client ) );
		else if ( IsRoleKnown )
			SendRole( To.Single( client ) );
	}
}
