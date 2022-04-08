using Sandbox;

namespace TTT;

public partial class Player
{
	public new Corpse Corpse
	{
		get => base.Corpse as Corpse;
		set => base.Corpse = value;
	}

	/// <summary>
	/// The player who confirmed this player's corpse.
	/// </summary>
	public Player Confirmer { get; set; }

	public int CorpseCredits { get; set; } = 0;

	public string LastSeenPlayerName { get; private set; }
	public bool IsRoleKnown { get; set; } = false;
	public bool IsConfirmedDead { get; set; } = false;
	public bool IsMissingInAction { get; set; } = false;

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
			Position = Position,
			Rotation = Rotation
		};

		corpse.CopyFrom( this );
		corpse.ApplyForceToBone( LastDamageInfo.Force, GetHitboxBone( LastDamageInfo.HitboxIndex ) );

		Corpse = corpse;
	}

	public void SyncMIA( Player player = null )
	{
		Host.AssertServer();

		if ( player is not null )
		{
			AddMIA( To.Single( player ) );
			return;
		}

		IsMissingInAction = true;
		AddMIA( Team.Traitors.ToClients() );
	}

	public void Confirm( To? _to = null )
	{
		Host.AssertServer();

		bool wasPreviouslyConfirmed = true;

		if ( !IsConfirmedDead )
		{
			IsConfirmedDead = true;
			IsRoleKnown = true;
			IsMissingInAction = false;
			wasPreviouslyConfirmed = false;
		}

		var to = _to ?? To.Everyone;

		SendRole( to );

		if ( Corpse.IsValid() )
			Corpse.SendInfo( to );

		ClientConfirm( to, Confirmer, wasPreviouslyConfirmed );
	}

	private void CheckLastSeenPlayer()
	{
		var trace = Trace.Ray( Owner.EyePosition, EyeRotation.Forward * HintDistance )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.UseHitboxes()
			.Run();

		if ( trace.Entity is Player player && player.CanHint( this ) )
			LastSeenPlayerName = player.Client?.Name;
	}

	[ClientRpc]
	private void ClientConfirm( Player confirmer, bool wasPreviouslyConfirmed = false )
	{
		Confirmer = confirmer;
		IsConfirmedDead = true;
		IsMissingInAction = false;

		if ( wasPreviouslyConfirmed || !Confirmer.IsValid() || !Corpse.IsValid() )
			return;

		UI.InfoFeed.Instance.AddClientToClientEntry
		(
			Confirmer.Client,
			Corpse.PlayerName,
			Role.Color,
			"found the body of",
			$"({Role.Title})"
		);
	}

	[ClientRpc]
	private void AddMIA()
	{
		IsMissingInAction = true;
	}
}
