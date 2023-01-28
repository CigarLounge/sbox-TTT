using Sandbox;
using Sandbox.UI;
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

	public const string BeepSound = "health_station-beep";
	private const float HealAmount = 1; // The amount of health given per usage.
	private const float TimeUntilNextHeal = 0.2f; // The time before we give out another heal.
	private const float RechargeAmount = 0.5f; // The amount of health recharged per second.

	private PointLightEntity _usageLight;
	private UI.HealthStationCharges _healthStationCharges;
	private RealTimeSince _timeSinceLastUsage;
	private RealTimeUntil _isHealAvailable;

	public override void Spawn()
	{
		base.Spawn();

		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 200f;

		_usageLight = new PointLightEntity
		{
			Enabled = false,
			DynamicShadows = true,
			Range = 13,
			Brightness = 20,
			Color = new Color32( 47, 106, 127 ),
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
		_usageLight.SetLightEnabled( _timeSinceLastUsage < TimeUntilNextHeal * 2 );

		if ( StoredHealth >= 200f )
			return;

		StoredHealth = Math.Min( StoredHealth + RechargeAmount * Time.Delta, 200f );
	}

	private void HealPlayer( Player player )
	{
		if ( StoredHealth <= 0 )
			return;

		var healthNeeded = Player.MaxHealth - player.Health;
		if ( healthNeeded <= 0 || !_isHealAvailable )
			return;

		_timeSinceLastUsage = 0f;
		_isHealAvailable = TimeUntilNextHeal;
		PlaySound( BeepSound );

		var healAmount = Math.Min( HealAmount, healthNeeded );
		player.Health += healAmount;
		StoredHealth -= healAmount;
	}

	Panel IEntityHint.DisplayHint( Player player ) => new UI.HealthStationHint( this );

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		HealPlayer( player );

		return player.Health < Player.MaxHealth && StoredHealth > 0;
	}

	bool IUse.IsUsable( Entity user )
	{
		if ( StoredHealth <= 0 )
			return false;

		if ( user is not Player player || !player.IsAlive || player.Health >= Player.MaxHealth )
			return false;

		return true;
	}
}
