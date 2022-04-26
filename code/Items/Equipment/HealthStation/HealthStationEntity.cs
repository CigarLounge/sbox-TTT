using Sandbox;
using System;

namespace TTT;

[Library( "ttt_entity_healthstation", Title = "Health Station" )]
[Hammer.EditorModel( "models/health_station/health_station.vmdl" )]
public partial class HealthStationEntity : Prop, IEntityHint, IUse
{
	private static readonly Model WorldModel = Model.Load( "models/health_station/health_station.vmdl" );

	[Net]
	public float StoredHealth { get; set; } = 200f;

	private const float HealAmount = 5; // The amount of health given per second.
	private const float RechargeAmount = 0.5f; // The amount of health recharged per second.

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 201f;
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( StoredHealth >= 200f )
			return;

		StoredHealth = Math.Min( StoredHealth + RechargeAmount * Time.Delta, 200f );
	}

	private void HealPlayer( Player player )
	{
		if ( StoredHealth <= 0 )
			return;
	
		float healthNeeded = player.MaxHealth - player.Health;

		if ( healthNeeded <= 0 )
			return;

		float healAmount = Math.Min( StoredHealth, Math.Min( HealAmount * Time.Delta, healthNeeded ) );

		player.Health += healAmount;

		StoredHealth -= healAmount;
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
