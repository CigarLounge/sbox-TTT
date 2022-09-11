using Sandbox;
using System;

namespace TTT;

[ClassName( "ttt_entity_healthstation" )]
[EditorModel( "models/health_station/health_station.vmdl" )]
[Title( "Health Station" )]
public partial class HealthStationEntity : Prop, IEntityHint, IUse
{
	private static readonly Model _worldModel = Model.Load( "models/health_station/health_station.vmdl" );

	[Net]
	public float StoredHealth { get; set; } = 200f;

	private const float HealAmount = 5; // The amount of health given per second.
	private const float RechargeAmount = 0.5f; // The amount of health recharged per second.

	private readonly Color _usageColor = new Color32( 173, 216, 230 );
	private PointLightEntity _usageLight;
	private UI.HealthStationCharges _healthStationCharges;
	private TimeSince _timeSinceLastUsage;

	public override void Spawn()
	{
		base.Spawn();

		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 200f;

		_usageLight = new PointLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 13,
			Brightness = 20,
			Color = Color.Transparent,
			Owner = this,
			Parent = this,
			Rotation = Rotation,
		};
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		_healthStationCharges = new( this );
	}

	protected override void OnDestroy()
	{
		_usageLight?.Delete();
		_healthStationCharges?.Delete( true );

		base.OnDestroy();
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		_usageLight.Color = _timeSinceLastUsage < 0.1f ? _usageColor : Color.Transparent;

		if ( StoredHealth >= 200f )
			return;

		StoredHealth = Math.Min( StoredHealth + RechargeAmount * Time.Delta, 200f );
	}

	private void HealPlayer( Player player )
	{
		if ( StoredHealth <= 0 )
			return;

		var healthNeeded = Player.MaxHealth - player.Health;

		if ( healthNeeded <= 0 )
			return;

		var healAmount = Math.Min( StoredHealth, Math.Min( HealAmount * Time.Delta, healthNeeded ) );

		player.Health += healAmount;

		StoredHealth -= healAmount;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.HealthStationHint( this );

	bool IUse.OnUse( Entity user )
	{
		_timeSinceLastUsage = 0f;

		var player = user as Player;
		HealPlayer( player );

		return player.Health < Player.MaxHealth && StoredHealth > 0;
	}

	bool IUse.IsUsable( Entity user )
	{
		if ( StoredHealth <= 0 )
			return false;

		if ( user is not Player player || player.Health >= Player.MaxHealth )
			return false;

		return true;
	}
}
