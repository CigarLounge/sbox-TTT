using Sandbox;
using System;

namespace TTT;

[Hammer.EditorModel( "models/health_station/health_station.vmdl" )]
[Library( "ttt_entity_healthstation", Title = "Health Station" )]
public partial class HealthStationEntity : DroppableEntity, IEntityHint, IUse
{
	[Net]
	public float StoredHealth { get; set; } = 200f;

	private const string WorldModel = "models/health_station/health_station.vmdl";
	private const float HEALAMOUNT = 5; // The amount of health given per second.
	private const float RECHARGEAMOUNT = 0.5f; // The amount of health recharged per second.

	private TimeUntil _timeUntilRecharge;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 201f;
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( StoredHealth >= 200f || !_timeUntilRecharge )
			return;

		StoredHealth = Math.Min( StoredHealth + RECHARGEAMOUNT * Time.Delta, 200f );
	}

	private void HealPlayer( Player player )
	{
		if ( StoredHealth <= 0 )
			return;

		float healthNeeded = player.MaxHealth - player.Health;

		if ( healthNeeded <= 0 )
			return;

		float healAmount = Math.Min( StoredHealth, Math.Min( HEALAMOUNT * Time.Delta, healthNeeded ) );

		player.SetHealth( player.Health + healAmount );

		StoredHealth -= healAmount;
		_timeUntilRecharge = 10;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.HealthStationHint( this );

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		HealPlayer( player );

		return player.Health < player.MaxHealth && StoredHealth > 0;
	}

	bool IUse.IsUsable( Entity user )
	{
		if ( StoredHealth <= 0 )
			return false;

		if ( user is not Player player || player.Health >= player.MaxHealth )
			return false;

		return true;
	}
}
