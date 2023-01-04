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
	public long SteamId { get; internal set; }

	[Net]
	public string SteamName { get; internal set; }

	[Net, Local]
	public Corpse Corpse { get; internal set; }
	/// <summary>
	/// The player who confirmed this player's death.
	/// </summary>
	public Player Confirmer { get; private set; }
	public bool IsAlive => Status == PlayerStatus.Alive;
	public bool IsMissingInAction => Status == PlayerStatus.MissingInAction;
	public bool IsConfirmedDead => Status == PlayerStatus.ConfirmedDead;
	public Player LastSeenPlayer { get; internal set; }
	public List<Player> PlayersKilled { get; internal set; } = new();

	private string _lastWords;
	private TimeSince _timeSinceLastWords;
	public string LastWords
	{
		get
		{
			if ( _timeSinceLastWords > 3 )
				_lastWords = string.Empty;

			return _lastWords;
		}
		set
		{
			if ( !IsAlive )
				return;

			_timeSinceLastWords = 0;
			_lastWords = value;
		}
	}

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

			Event.Run( GameEvent.Player.StatusChanged, this, oldStatus );
		}
	}

	public void RemoveCorpse()
	{
		Game.AssertServer();

		Corpse?.Delete();
		Corpse = null;
	}

	protected void BecomeCorpse()
	{
		Game.AssertServer();

		Corpse = new Corpse( this );
	}

	public void ConfirmDeath( Player confirmer = null )
	{
		Game.AssertServer();

#if DEBUG
		if ( IsAlive || IsSpectator )
		{
			Log.Error( "Trying to confirm an alive player or spectator!" );
			return;
		}

		if ( IsConfirmedDead )
		{
			Log.Error( "This player is already confirmed dead!" );
			return;
		}
#endif

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
		Game.AssertServer();

		if ( IsSpectator )
			return;

		IsRoleKnown = true;

		if ( IsMissingInAction )
			ConfirmDeath();

		if ( Corpse.IsValid() && !Corpse.IsFound )
		{
			Corpse.IsFound = true;
			Corpse.SendPlayer( To.Everyone );
			Corpse.ClientCorpseFound( To.Everyone, null );
		}
	}

	/// <summary>
	/// If the player is <strong><see cref="PlayerStatus.MissingInAction"/></strong>,
	/// update the status for the client owner 
	/// and <strong><see cref="Team.Traitors"/></strong>.
	/// </summary>
	public void UpdateMissingInAction()
	{
		Game.AssertServer();

#if DEBUG
		if ( !IsMissingInAction )
		{
			Log.Error( $"{SteamName} is not MIA!" );
			return;
		}
#endif

		UpdateStatus( Team.Traitors.ToClients() );

		if ( Team != Team.Traitors )
			UpdateStatus( To.Single( this ) );
	}

	public void UpdateStatus( To to )
	{
		Game.AssertServer();

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

	[GameEvent.Client.Joined]
	private void SyncClient( IClient client )
	{
		if ( IsRoleKnown )
			ClientSetRole( To.Single( client ), Role.Info );

		if ( IsSpectator )
			UpdateStatus( To.Single( client ) );
		else if ( IsConfirmedDead )
			ClientConfirmDeath( To.Single( client ), Confirmer );

		if ( Corpse.IsValid() && Corpse.IsFound )
		{
			Corpse.SendPlayer( To.Single( client ) );
			Corpse.ClientCorpseFound( To.Single( client ), Corpse.Finder, true );
		}
	}
}
