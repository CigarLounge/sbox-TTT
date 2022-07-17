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

	public Corpse Corpse { get; set; }
	/// <summary>
	/// The player who confirmed this player's death.
	/// </summary>
	public Player Confirmer { get; private set; }
	public bool IsMissingInAction => Status == PlayerStatus.MissingInAction;
	public bool IsConfirmedDead => Status == PlayerStatus.ConfirmedDead;
	public Player LastSeenPlayer { get; set; }
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

	public void ConfirmDeath( Player confirmer = null )
	{
		Host.AssertServer();

		if ( this.IsAlive() || IsSpectator )
		{
			Log.Warning( "Trying to confirm an alive player or spectator!" );
			return;
		}

		if ( IsConfirmedDead )
		{
			Log.Warning( "This player is already confirmed dead!" );
			return;
		}

		Confirmer = confirmer;
		Status = PlayerStatus.ConfirmedDead;
		ClientConfirmDeath( confirmer );
	}

	/// <summary>
	/// Reveals the player's role.
	/// If the player is MIA, confirm his death and send the player's corpse to everyone.
	/// </summary>
	public void Reveal()
	{
		Host.AssertServer();

		if ( IsSpectator )
			return;

		IsRoleKnown = true;

		if ( IsMissingInAction )
			ConfirmDeath();

		if ( Corpse.IsValid() && !Corpse.IsFound )
		{
			Corpse.IsFound = true;
			Corpse.SendPlayer( To.Everyone );
		}
	}

	public void UpdateMissingInAction()
	{
		Host.AssertServer();

		if ( !IsMissingInAction )
			return;

		UpdateStatus( Team.Traitors.ToClients() );

		if ( Team != Team.Traitors )
			UpdateStatus( To.Single( this ) );
	}

	public void UpdateStatus( To to )
	{
		Host.AssertServer();

		ClientSetStatus( to, Status );
	}

	private void CheckLastSeenPlayer()
	{
		if ( HoveredEntity is Player player && player.CanHint( this ) )
			LastSeenPlayer = player;
	}

	private void ResetConfirmationData()
	{
		Confirmer = null;
		Corpse = null;
		LastSeenPlayer = null;
		PlayersKilled.Clear();
	}

	[ClientRpc]
	private void ClientConfirmDeath( Player confirmer )
	{
		Confirmer = confirmer;
		Status = PlayerStatus.ConfirmedDead;
	}

	[ClientRpc]
	private void ClientSetStatus( PlayerStatus status )
	{
		Status = status;
	}

	[TTTEvent.Game.ClientJoined]
	private void SyncClient( Client client )
	{
		if ( IsRoleKnown )
			SendRole( To.Single( client ) );

		if ( IsSpectator )
			UpdateStatus( To.Single( client ) );
		else if ( IsConfirmedDead )
			ClientConfirmDeath( To.Single( client ), Confirmer );
	}
}
