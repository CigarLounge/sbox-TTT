using Sandbox;

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
	public Corpse Corpse { get; set; }

	/// <summary>
	/// The player who confirmed this player's corpse.
	/// </summary>
	public Player Confirmer { get; private set; }
	public PlayerStatus Status { get; set; } = PlayerStatus.Spectator;
	public bool IsMissingInAction => Status == PlayerStatus.MissingInAction;
	public bool IsConfirmedDead => Status == PlayerStatus.ConfirmedDead;
	public bool IsRoleKnown { get; set; }
	public string LastSeenPlayerName { get; set; }

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
			SetStatusRPC( To.Single( player ), PlayerStatus.MissingInAction );
			return;
		}

		Status = PlayerStatus.MissingInAction;
		SetStatusRPC( Team.Traitors.ToClients(), PlayerStatus.MissingInAction );

		if ( Team != Team.Traitors )
			SetStatusRPC( To.Single( this ), PlayerStatus.MissingInAction );
	}

	public void Confirm( To to, Player confirmer = null )
	{
		Host.AssertServer();

		var wasPreviouslyConfirmed = true;

		if ( !IsConfirmedDead )
		{
			Confirmer = confirmer;
			Status = PlayerStatus.ConfirmedDead;
			IsRoleKnown = true;
			wasPreviouslyConfirmed = false;
		}

		if ( Corpse.IsValid() )
			Corpse.SendPlayer( to );

		SendRole( to );
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
		LastSeenPlayerName = string.Empty;
		IsRoleKnown = false;
	}

	[ClientRpc]
	private void ClientConfirm( Player confirmer, bool wasPreviouslyConfirmed = false )
	{
		Confirmer = confirmer;
		Status = PlayerStatus.ConfirmedDead;

		if ( wasPreviouslyConfirmed || !Confirmer.IsValid() || !Corpse.IsValid() )
			return;

		Event.Run( TTTEvent.Player.CorpseFound, this );
	}

	[ClientRpc]
	private void SetStatusRPC( PlayerStatus someState )
	{
		Status = someState;
	}

	[TTTEvent.Game.ClientJoined]
	private void SyncClient( Client client )
	{
		if ( this.IsAlive() )
			SetStatusRPC( To.Single( client ), PlayerStatus.Alive );

		if ( IsConfirmedDead )
			Confirm( To.Single( client ) );
		else if ( IsRoleKnown )
			SendRole( To.Single( client ) );
	}
}
