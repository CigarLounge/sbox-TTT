using Sandbox;
using System.Collections.Generic;

namespace TTT;

[ClassName( "ttt_entity_corpse" )]
[HideInEditor]
[Title( "Player corpse" )]
public partial class Corpse : ModelEntity, IEntityHint, IUse
{
	[Net]
	public bool HasCredits { get; private set; }

	public Player Player { get; set; }
	public TimeUntil TimeUntilDNADecay { get; private set; }
	public string C4Note { get; private set; }
	public PerkInfo[] Perks { get; private set; }
	public string[] KillList { get; private set; }

	// Clientside only
	public bool HasCalledDetective { get; set; } = false;

	public List<Particles> Ropes { get; private set; } = new();
	public List<PhysicsJoint> RopeSprings { get; private set; } = new();

	// We need this so we don't send information to players multiple times
	// The HashSet consists of NetworkIds
	private readonly HashSet<int> _playersWhoGotKillInfo = new();

	public Corpse() { }

	public Corpse( Player player )
	{
		Host.AssertServer();

		Player = player;
		HasCredits = player.Credits > 0;
		Transform = player.Transform;

		if ( Player.LastDamage.Flags == DamageFlags.Bullet && Player.LastAttacker is Player killer )
		{
			var dna = new DNA( killer );
			Components.Add( dna );
			TimeUntilDNADecay = dna.TimeUntilDecayed;
		}

		var c4Note = player.Components.Get<C4Note>();
		if ( c4Note is not null )
			C4Note = c4Note.SafeWireNumber.ToString();

		Perks = new PerkInfo[Player.Perks.Count];
		for ( var i = 0; i < Player.Perks.Count; i++ )
			Perks[i] = Player.Perks[i].Info;

		KillList = new string[Player.PlayersKilled.Count];
		for ( var i = 0; i < Player.PlayersKilled.Count; i++ )
			KillList[i] = Player.PlayersKilled[i].SteamName;

		SetModel( player.GetModelName() );
		TakeDecalsFrom( player );

		this.CopyBonesFrom( player );
		this.SetRagdollVelocityFrom( player );
		ApplyForceToBone( Player.LastDamage.Force, Player.GetHitboxBone( Player.LastDamage.HitboxIndex ) );

		foreach ( var clothing in Player.Clothes.ToArray() )
			clothing.SetParent( this, true );

		Player.SetClothingBodyGroups( this, 1 );

		_playersWhoGotKillInfo.Add( player.NetworkIdent );
	}

	public override void Spawn()
	{
		base.Spawn();

		MoveType = MoveType.Physics;
		UsePhysicsCollision = true;

		SetInteractsAs( CollisionLayer.Debris );
		SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		SetInteractsExclude( CollisionLayer.Player );
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

		var creditsRetrieved = 0;
		canRetrieveCredits &= searcher.Role.CanRetrieveCredits & searcher.IsAlive();

		if ( canRetrieveCredits && HasCredits )
		{
			searcher.Credits += Player.Credits;
			creditsRetrieved = Player.Credits;
			Player.Credits = 0;
			HasCredits = false;
		}

		Player.Confirm( To.Single( searcher ) );
		SendKillInfo( To.Single( searcher ) );
		ClientSearch( To.Single( searcher ), creditsRetrieved );

		// Dead players will always covert search.
		covert |= !searcher.IsAlive();

		if ( !covert )
		{
			if ( !Player.IsConfirmedDead )
			{
				Player.Confirm( To.Everyone, true, searcher );

				foreach ( var deadPlayer in Player.PlayersKilled )
					deadPlayer?.Confirm( To.Everyone, true, searcher );

				Event.Run( TTTEvent.Player.CorpseFound, Player );
			}

			// If the searcher is a detective, send kill info to everyone.
			if ( searcher.Role is Detective )
				SendKillInfo( To.Everyone );
		}
	}

	public void SendKillInfo( To to )
	{
		Host.AssertServer();

		foreach ( var client in to )
		{
			if ( _playersWhoGotKillInfo.Contains( client.Pawn.NetworkIdent ) )
				continue;

			_playersWhoGotKillInfo.Add( client.Pawn.NetworkIdent );

			Player.SendDamageInfo( To.Single( client ) );

			SendMiscInfo( KillList, C4Note, TimeUntilDNADecay );

			if ( client.Pawn is Player player && player.Role is Detective )
				SendDetectiveInfo( To.Single( client ), Player.LastSeenPlayerName );
		}
	}

	[ClientRpc]
	private void SendMiscInfo( string[] killList, string c4Note, TimeUntil dnaDecay )
	{
		KillList = killList;
		C4Note = c4Note;
		TimeUntilDNADecay = dnaDecay;
	}

	/// <summary>
	/// Detectives get additional information about a corpse.
	/// </summary>
	[ClientRpc]
	private void SendDetectiveInfo( string lastSeenPlayerName )
	{
		Player.LastSeenPlayerName = lastSeenPlayerName;
	}

	[ClientRpc]
	private void ClientSearch( int creditsRetrieved = 0 )
	{
		if ( creditsRetrieved <= 0 )
			return;

		UI.InfoFeed.AddEntry
		(
			Local.Client,
			$"found {creditsRetrieved} credits!"
		);
	}

	public void RemoveRopeAttachments()
	{
		foreach ( var rope in Ropes )
			rope.Destroy( true );

		foreach ( var spring in RopeSprings )
			spring.Remove();

		Ropes.Clear();
		RopeSprings.Clear();
	}

	protected override void OnDestroy()
	{
		RemoveRopeAttachments();

		base.OnDestroy();
	}

	private void ApplyForceToBone( Vector3 force, int forceBone )
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


	float IEntityHint.HintDistance => Player.MaxHintDistance;

	bool IEntityHint.CanHint( Player player ) => Game.Current.State is InProgress or PostRound;

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.CorpseHint( this );

	void IEntityHint.Tick( Player player )
	{
		if ( !Player.IsValid() || !player.IsLocalPawn || !CanSearch() || !Input.Down( GetSearchButton() ) )
			UI.FullScreenHintMenu.Instance?.Close();
		else if ( !Player.LastDamage.Equals( default( DamageInfo ) ) && !UI.FullScreenHintMenu.Instance.IsOpen )
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

		if ( searchButton == InputButton.PrimaryAttack )
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

		return InputButton.PrimaryAttack;
	}
}
