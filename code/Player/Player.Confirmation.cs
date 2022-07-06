using Sandbox;

namespace TTT;

public partial class Player
{
	public Corpse Corpse { get; set; }

	/// <summary>
	/// The player who confirmed this player's corpse.
	/// </summary>
	public Player Confirmer { get; private set; }

	public bool IsRoleKnown { get; set; } = false;
	public bool IsConfirmedDead { get; set; } = false;
	public bool IsMissingInAction { get; set; } = false;

	public string LastSeenPlayerName { get; set; }

	public void RemoveCorpse()
	{
		if ( !IsServer || !Corpse.IsValid() )
			return;

		Corpse.Delete();
		Corpse = null;
	}

	private void BecomeCorpse()
	{
		Host.AssertServer();

		var corpse = new Corpse()
		{
			Transform = Transform
		};

		corpse.CopyFrom( this );
		corpse.ApplyForceToBone( LastDamageInfo.Force, GetHitboxBone( LastDamageInfo.HitboxIndex ) );

		Corpse = corpse;
	}

	public void UpdateMissingInAction( Player player = null )
	{
		Host.AssertServer();

		if ( player is not null )
		{
			ClientMissingInAction( To.Single( player ) );
			return;
		}

		IsMissingInAction = true;
		ClientMissingInAction( Team.Traitors.ToClients() );
	}

	public void Confirm( To to, Player confirmer = null )
	{
		Host.AssertServer();

		var wasPreviouslyConfirmed = true;

		if ( !IsConfirmedDead )
		{
			Confirmer = confirmer;
			IsConfirmedDead = true;
			IsRoleKnown = true;
			IsMissingInAction = false;
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
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;
	}

	[ClientRpc]
	private void ClientConfirm( Player confirmer, bool wasPreviouslyConfirmed = false )
	{
		Confirmer = confirmer;
		IsConfirmedDead = true;
		IsMissingInAction = false;

		if ( wasPreviouslyConfirmed || !Confirmer.IsValid() || !Corpse.IsValid() )
			return;

		Event.Run( TTTEvent.Player.CorpseFound, this );
	}

	[ClientRpc]
	private void ClientMissingInAction()
	{
		IsMissingInAction = true;
	}
}
