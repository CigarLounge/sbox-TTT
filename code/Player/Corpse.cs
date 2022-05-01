using Sandbox;
using System.Collections.Generic;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_entity_corpse", Title = "Corpse" )]
public partial class Corpse : ModelEntity, IEntityHint, IUse
{
	[Net]
	public bool HasCredits { get; private set; }

	public long PlayerId { get; private set; }
	public string PlayerName { get; private set; }
	public Player Player { get; private set; }
	public DamageInfo KillInfo { get; private set; }
	public CarriableInfo KillerWeapon { get; private set; }
	public bool WasHeadshot => GetHitboxGroup( KillInfo.HitboxIndex ) == (int)HitboxGroup.Head;
	public float KilledTime { get; private set; }
	public string C4Note { get; private set; }
	public PerkInfo[] Perks { get; private set; }

	// Detective information
	public string LastSeenPlayerName { get; private set; }

	public List<Particles> Ropes = new();
	public List<PhysicsJoint> RopeSprings = new();

	// Clientside only
	public bool HasCalledDetective { get; set; } = false;

	// We need this so we don't send information to players multiple times
	// The HashSet consists of NetworkIds
	private readonly HashSet<int> _playersWhoGotKillInfo = new();
	private readonly HashSet<int> _playersWhoGotPlayerData = new();

	// Only display the inspect menu if this is true.
	private bool _receivedKillInfo;

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

		Player = player;
		PlayerName = player.Client.Name;
		PlayerId = player.Client.PlayerId;
		KillInfo = player.LastDamageInfo;
		KillerWeapon = Asset.GetInfo<CarriableInfo>( KillInfo.Weapon );
		HasCredits = player.Credits > 0;

		var c4Note = player.Components.Get<C4Note>();
		if ( c4Note is not null )
			C4Note = c4Note.SafeWireNumber.ToString();

		LastSeenPlayerName = player.LastSeenPlayerName;

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

		Perks = new PerkInfo[Player.Perks.Count];
		for ( int i = 0; i < Player.Perks.Count; i++ )
		{
			Perks[i] = Player.Perks[i].Info;
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
	/// Search this corpse.
	/// </summary>
	/// <param name="searcher">The player who is searching this corpse.</param>
	/// <param name="covert">Whether or not this is a covert search.</param>
	/// <param name="canRetrieveCredits">Should the searcher retrieve credits.</param>
	public void Search( Player searcher, bool covert, bool canRetrieveCredits = true )
	{
		Host.AssertServer();

		int creditsRetrieved = 0;
		canRetrieveCredits &= searcher.Role.CanRetrieveCredits & searcher.IsAlive();

		if ( canRetrieveCredits && HasCredits )
		{
			searcher.Credits += Player.Credits;
			creditsRetrieved = Player.Credits;
			Player.Credits = 0;
			HasCredits = false;
		}

		SendPlayer( To.Single( searcher ) );
		Player.SendRole( To.Single( searcher ) );
		SendKillInfo( To.Single( searcher ) );

		// Dead players will always covert search.
		covert |= !searcher.IsAlive();

		if ( !covert )
		{
			if ( !Player.IsConfirmedDead )
			{
				Player.Confirm( To.Everyone, searcher );

				Event.Run( TTTEvent.Player.CorpseFound, Player );
			}

			// If the searcher is a detective, send kill info to everyone.
			if ( searcher.Role is Detective )
				SendKillInfo( To.Everyone );
		}

		ClientSearch( To.Single( searcher ), creditsRetrieved );
	}

	[ClientRpc]
	private void ClientSearch( int creditsRetrieved = 0 )
	{
		Player.IsMissingInAction = !Player.IsConfirmedDead;

		if ( creditsRetrieved <= 0 )
			return;

		UI.InfoFeed.Instance?.AddEntry
		(
			Local.Client,
			$"found {creditsRetrieved} credits!"
		);
	}

	public void SendKillInfo( To to )
	{
		Host.AssertServer();

		foreach ( var client in to )
		{
			if ( _playersWhoGotKillInfo.Contains( client.Pawn.NetworkIdent ) )
				continue;

			_playersWhoGotKillInfo.Add( client.Pawn.NetworkIdent );

			SendKillInfo
			(
				To.Single( client ),
				KillInfo.Attacker,
				KillerWeapon,
				KillInfo.HitboxIndex,
				KillInfo.Damage,
				KillInfo.Flags,
				KilledTime
			);

			SendMiscInfo
			(
				C4Note
			);

			if ( client.Pawn is Player player && player.Role is Detective )
				DetectiveSendKillInfo( To.Single( client ), LastSeenPlayerName );
		}
	}

	[ClientRpc]
	private void SendKillInfo( Entity attacker, CarriableInfo killerWeapon, int hitboxIndex, float damage, DamageFlags damageFlag, float killedTime )
	{
		var info = new DamageInfo()
			.WithAttacker( attacker )
			.WithHitbox( hitboxIndex )
			.WithFlag( damageFlag );

		info.Damage = damage;
		KillInfo = info;
		LastAttacker = info.Attacker;
		KillerWeapon = killerWeapon;
		KilledTime = killedTime;

		_receivedKillInfo = true;
	}

	[ClientRpc]
	private void SendMiscInfo( string c4Note )
	{
		C4Note = c4Note;
	}

	/// <summary>
	/// Detectives get additional information about a corpse.
	/// </summary>
	[ClientRpc]
	private void DetectiveSendKillInfo( string lastSeenPlayerName )
	{
		LastSeenPlayerName = lastSeenPlayerName;
	}

	public void SendPlayer( To to )
	{
		foreach ( var client in to )
		{
			if ( _playersWhoGotPlayerData.Contains( client.Pawn.NetworkIdent ) )
				continue;

			_playersWhoGotPlayerData.Add( client.Pawn.NetworkIdent );

			SendPlayer( To.Single( client ), Player, PlayerId, PlayerName, Perks );
		}
	}

	[ClientRpc]
	private void SendPlayer( Player deadPlayer, long playerId, string name, PerkInfo[] perks )
	{
		Player = deadPlayer;
		PlayerId = playerId;
		PlayerName = name;
		Perks = perks;
		Player.Corpse = this;
	}

	float IEntityHint.HintDistance => Player.MaxHintDistance;

	bool IEntityHint.CanHint( Player player ) => Game.Current.State is InProgress or PostRound;

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.CorpseHint( this );

	void IEntityHint.Tick( Player player )
	{
		if ( !player.IsLocalPawn || !CanSearch() || !Input.Down( GetSearchButton() ) )
			UI.FullScreenHintMenu.Instance?.Close();
		else if ( _receivedKillInfo && !UI.FullScreenHintMenu.Instance.IsOpen )
			UI.FullScreenHintMenu.Instance?.Open( new UI.InspectMenu( this ) );
	}

	bool IUse.OnUse( Entity user ) => true;

	bool IUse.IsUsable( Entity user )
	{
		if ( user is not Player player )
			return false;

		if ( Game.Current.State is WaitingState or PreRound )
			return false;

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
