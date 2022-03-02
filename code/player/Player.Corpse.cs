using System.Collections.Generic;

using Sandbox;

namespace TTT;

public struct ClientData
{

}

public partial class Corpse : ModelEntity, IEntityHint, IUse
{
	public long PlayerId { get; private set; }
	public string PlayerName { get; private set; }
	public Player DeadPlayer { get; private set; }
	public Player Confirmer { get; private set; }
	public DamageInfo KillInfo { get; set; }
	public List<Particles> Ropes = new();
	public List<PhysicsJoint> RopeSprings = new();
	public CarriableInfo KillerWeapon { get; private set; }
	public bool WasHeadshot => GetHitboxGroup( KillInfo.HitboxIndex ) == (int)HitboxGroup.Head;
	public float Distance { get; private set; } = 0f;
	public float KilledTime { get; private set; }
	public string[] Perks { get; set; }

	public Corpse() { }

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
		foreach ( Entity child in player.Children )
		{
			// TODO: MZEGAR revert this back.
			// if ( child is ThrownKnife k )
			// {
			// 	attachedEnts.Add( k );
			// 	continue;
			// }

			if ( child is BaseClothing e )
			{
				var model = e.GetModelName();
				var clothing = new ModelEntity();
				clothing.RenderColor = e.RenderColor;
				clothing.SetModel( model );
				clothing.SetParent( this, true );
			}
		}

		Perks = new string[DeadPlayer.Perks.Count];

		for ( int i = 0; i < DeadPlayer.Perks.Count; i++ )
		{
			Perks[i] = DeadPlayer.Perks.Get( i ).Info.Title;
		}

		foreach ( Entity entity in attachedEnts )
		{
			entity.SetParent( this, false );
		}
	}

	public void ApplyForceToBone( Vector3 force, int forceBone )
	{
		PhysicsGroup.AddVelocity( force );

		if ( forceBone < 0 )
			return;

		PhysicsBody corpse = GetBonePhysicsBody( forceBone );

		if ( corpse != null )
			corpse.ApplyForce( force * 1000 );
		else
			PhysicsGroup.AddVelocity( force );
	}

	public void ClearAttachments()
	{
		foreach ( Particles rope in Ropes )
		{
			rope.Destroy( true );
		}

		foreach ( PhysicsJoint spring in RopeSprings )
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

	[ClientRpc]
	public void GetDamageInfo( Entity attacker, int weaponId, int hitboxIndex, float damage, DamageFlags damageFlag )
	{
		var info = new DamageInfo()
			.WithAttacker( attacker )
			.WithHitbox( hitboxIndex )
			.WithFlag( damageFlag );

		info.Damage = damage;
		KillInfo = info;
		LastAttacker = info.Attacker;
		KillerWeapon = Asset.FromId<CarriableInfo>( weaponId );
	}

	[ClientRpc]
	public void GetPlayerData( Player deadPlayer, Player confirmer, float killedTime, float distance, long playerId, string name, int credits = 0 )
	{
		DeadPlayer = deadPlayer;
		DeadPlayer.IsConfirmedDead = true;
		DeadPlayer.IsRoleKnown = true;
		DeadPlayer.IsMissingInAction = false;
		Confirmer = confirmer;
		KilledTime = killedTime;
		Distance = distance;
		PlayerId = playerId;
		PlayerName = name;

		UI.Scoreboard.Instance.UpdateClient( DeadPlayer.Client );

		if ( confirmer == null )
			return;

		UI.InfoFeed.Instance.AddEntry
		(
			Confirmer.Client,
			PlayerName,
			deadPlayer.Role.Info.Color,
			"found the body of",
			$"({deadPlayer.Role.Info.Title})"
		);

		if ( Confirmer == Local.Pawn && credits > 0 )
		{
			UI.InfoFeed.Instance?.AddEntry
			(
				Confirmer.Client,
				$"found $ {credits} credits!"
			);
		}
	}

	public void Confirm()
	{
		Host.AssertServer();

		DeadPlayer.IsConfirmedDead = true;
		DeadPlayer.IsRoleKnown = true;
		DeadPlayer.IsMissingInAction = false;
		DeadPlayer.SendClientRole( To.Everyone );

		int credits = 0;
		if ( DeadPlayer.Credits > 0 && Confirmer.IsValid() && Confirmer.Role.Info.RetrieveCredits )
		{
			Confirmer.Credits += DeadPlayer.Credits;
			credits = DeadPlayer.Credits;
			DeadPlayer.Credits = 0;
			DeadPlayer.CorpseCredits = DeadPlayer.Credits;
		}

		GetDamageInfo( KillInfo.Attacker, KillerWeapon?.Id ?? 0, KillInfo.HitboxIndex, KillInfo.Damage, KillInfo.Flags );
		GetPlayerData( DeadPlayer, Confirmer, KilledTime, Distance, PlayerId, PlayerName, credits );
	}

	public float HintDistance => Player.INTERACT_DISTANCE;

	// DeadPlayer is only sent to client once the body is confirmed, therefore check if null.
	public string TextOnTick => DeadPlayer == null ? $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to inspect the corpse"
												   : $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to identify the corpse";

	bool IEntityHint.CanHint( Player client ) => true;

	UI.EntityHintPanel IEntityHint.DisplayHint( Player client )
	{
		return new UI.Hint( TextOnTick );
	}

	void IEntityHint.Tick( Player player )
	{
		if ( player.Using != this )
			UI.FullScreenHintMenu.Instance?.Close();
		else if ( DeadPlayer.IsConfirmedDead && !UI.FullScreenHintMenu.Instance.IsOpen )
			UI.FullScreenHintMenu.Instance?.Open( new UI.InspectMenu( this ) );
	}

	bool IUse.OnUse( Entity user )
	{
		if ( IsServer && !DeadPlayer.IsConfirmedDead && user.IsAlive() )
		{
			Confirmer = user as Player;
			Confirm();
		}

		return true;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user is Player;
	}
}
