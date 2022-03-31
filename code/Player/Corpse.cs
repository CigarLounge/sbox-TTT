using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Corpse : ModelEntity, IEntityHint, IUse
{
	public long PlayerId { get; private set; }
	public string PlayerName { get; private set; }
	public Player DeadPlayer { get; private set; }
	public DamageInfo KillInfo { get; private set; }
	public List<Particles> Ropes = new();
	public List<PhysicsJoint> RopeSprings = new();
	public CarriableInfo KillerWeapon { get; private set; }
	public bool WasHeadshot => GetHitboxGroup( KillInfo.HitboxIndex ) == (int)HitboxGroup.Head;
	public float Distance { get; private set; } = 0f;
	public float KilledTime { get; private set; }
	public PerkInfo[] Perks { get; private set; }

	// Clientside only
	public bool HasCalledDetective { get; set; } = false;

	// We need this so we don't send information to players multiple times
	// The HashSet consists of NetworkIds
	private readonly HashSet<int> _playersWhoGotSentInfo = new();

	public override void Spawn()
	{
		base.Spawn();

		MoveType = MoveType.Physics;
		UsePhysicsCollision = true;

		SetInteractsAs( CollisionLayer.Debris );
		SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		SetInteractsExclude( CollisionLayer.Player );

		KilledTime = Time.Now;
	}

	public void CopyFrom( Player player )
	{
		Host.AssertServer();

		DeadPlayer = player;
		PlayerName = player.Client.Name;
		PlayerId = player.Client.PlayerId;
		KillInfo = player.LastDamageInfo;
		KillerWeapon = Asset.GetInfo<CarriableInfo>( KillInfo.Weapon );
		Distance = player.LastDistanceToAttacker;
		player.Corpse = this;

		SetModel( player.GetModelName() );
		TakeDecalsFrom( player );

		this.CopyBonesFrom( player );
		this.SetRagdollVelocityFrom( player );

		List<Entity> attachedEnts = new();

		foreach ( var child in player.Children )
		{
			if ( child is BaseClothing e )
			{
				var model = e.GetModelName();
				var clothing = new ModelEntity();
				clothing.RenderColor = e.RenderColor;
				clothing.SetModel( model );
				clothing.SetParent( this, true );
			}
		}

		Player.SetClothingBodyGroups( this, 1 );

		Perks = new PerkInfo[DeadPlayer.Perks.Count];
		for ( int i = 0; i < DeadPlayer.Perks.Count; i++ )
		{
			Perks[i] = DeadPlayer.Perks.Get( i ).Info;
		}

		foreach ( var entity in attachedEnts )
		{
			entity.SetParent( this, false );
		}
	}

	public void ApplyForceToBone( Vector3 force, int forceBone )
	{
		PhysicsGroup.AddVelocity( force );

		if ( forceBone < 0 )
			return;

		var corpse = GetBonePhysicsBody( forceBone );

		if ( corpse is not null )
			corpse.ApplyForce( force * 1000 );
		else
			PhysicsGroup.AddVelocity( force );
	}

	public void ClearAttachments()
	{
		foreach ( var rope in Ropes )
		{
			rope.Destroy( true );
		}

		foreach ( var spring in RopeSprings )
		{
			spring.Remove();
		}

		Ropes.Clear();
		RopeSprings.Clear();
	}

	protected override void OnDestroy()
	{
		ClearAttachments();
	}

	/// <summary>
	/// Sends the <strong><see cref="TTT.Player"/></strong> this corpse belongs to alongside information about the player's death.
	/// </summary>
	/// <param name="to">The target.</param>
	public void SendInfo( To to )
	{
		Host.AssertServer();

		foreach ( var client in to )
		{
			// Don't send general data to players who already got info
			if ( _playersWhoGotSentInfo.Contains( client.Pawn.NetworkIdent ) )
				continue;

			_playersWhoGotSentInfo.Add( client.Pawn.NetworkIdent );

			GetKillInfo
			(
				To.Single( client ),
				KillInfo.Attacker,
				KillerWeapon,
				KillInfo.HitboxIndex,
				KillInfo.Damage,
				KillInfo.Flags,
				Distance,
				KilledTime
			);

			GetPlayer( To.Single( client ), DeadPlayer, PlayerId, PlayerName, Perks );
		}
	}

	/// <summary>
	/// Search this corpse.
	/// </summary>
	/// <param name="searcher">The player who is searching this corpse.</param>
	/// <param name="covert">Whether or not this is a covert search.</param>
	/// <param name="retrieveCredits">Should the searcher retrieve credits.</param>
	public void Search( Player searcher, bool covert, bool retrieveCredits = true )
	{
		Host.AssertServer();

		int credits = 0;
		retrieveCredits &= searcher.Role.RetrieveCredits & searcher.IsAlive();

		if ( DeadPlayer.Credits > 0 && retrieveCredits )
		{
			searcher.Credits += DeadPlayer.Credits;
			credits = DeadPlayer.Credits;
			DeadPlayer.Credits = 0;
			DeadPlayer.CorpseCredits = DeadPlayer.Credits;
		}

		if ( !covert && !DeadPlayer.IsConfirmedDead )
		{
			DeadPlayer.Confirmer = searcher;
			DeadPlayer.Confirm();
		}
		else if ( !_playersWhoGotSentInfo.Contains( searcher.NetworkIdent ) )
		{
			DeadPlayer.SendRole( To.Single( searcher ) );
			SendInfo( To.Single( searcher ) );
		}

		ClientSearch( To.Single( searcher ), credits );
	}

	[ClientRpc]
	private void ClientSearch( int credits = 0 )
	{
		DeadPlayer.IsMissingInAction = !DeadPlayer.IsConfirmedDead;

		if ( credits <= 0 )
			return;

		UI.InfoFeed.Instance?.AddClientEntry
		(
			Local.Client,
			$"found $ {credits} credits!"
		);
	}

	[ClientRpc]
	private void GetKillInfo( Entity attacker, CarriableInfo killerWeapon, int hitboxIndex, float damage, DamageFlags damageFlag, float distance, float killedTime )
	{
		var info = new DamageInfo()
			.WithAttacker( attacker )
			.WithHitbox( hitboxIndex )
			.WithFlag( damageFlag );

		info.Damage = damage;
		KillInfo = info;
		LastAttacker = info.Attacker;
		KillerWeapon = killerWeapon;
		Distance = distance;
		KilledTime = killedTime;
	}

	[ClientRpc]
	private void GetPlayer( Player deadPlayer, long playerId, string name, PerkInfo[] perks )
	{
		DeadPlayer = deadPlayer;
		PlayerId = playerId;
		PlayerName = name;
		Perks = perks;
		DeadPlayer.Corpse = this;
	}

	public float HintDistance { get; set; } = Player.MaxHintDistance;

	bool IEntityHint.CanHint( Player player ) => Game.Current.Round is InProgressRound or PostRound;

	UI.EntityHintPanel IEntityHint.DisplayHint( Player client ) => new UI.CorpseHint( this );

	void IEntityHint.Tick( Player player )
	{
		if ( !player.IsLocalPawn || !CanSearch() || !Input.Down( GetSearchButton() ) )
			UI.FullScreenHintMenu.Instance?.Close();
		else if ( DeadPlayer.IsValid() && !UI.FullScreenHintMenu.Instance.IsOpen )
			UI.FullScreenHintMenu.Instance?.Open( new UI.InspectMenu( this ) );
	}

	bool IUse.OnUse( Entity user ) => true;

	bool IUse.IsUsable( Entity user )
	{
		// For now, let's not let people inspect outside of InProgressRound.
		// we should probably create an "empty" corpse instead.
		if ( Game.Current.Round is not InProgressRound )
			return false;

		var player = user as Player;
		Search( player, Input.Down( InputButton.Run ) );

		return true;
	}

	public bool CanSearch()
	{
		Host.AssertClient();

		var searchButton = GetSearchButton();

		if ( searchButton == InputButton.Attack1 )
			return true;

		return CurrentView.Position.Distance( Position ) <= Player.UseDistance;
	}

	public static InputButton GetSearchButton()
	{
		Host.AssertClient();

		var player = Local.Pawn as Player;

		if ( player.ActiveChild is not Binoculars binoculars )
			return InputButton.Use;

		if ( !binoculars.IsZoomed )
			return InputButton.Use;

		return InputButton.Attack1;
	}
}
