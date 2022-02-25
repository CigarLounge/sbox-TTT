using System;

using Sandbox;

namespace TTT;

[Library( "ttt_equipment_healthstation_ent", Title = "Health Station" )]
public partial class HealthStationEntity : Prop, IEntityHint, IUse
{
	[Net]
	public float StoredHealth { get; set; } = 200f; // This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

	private const string _worldModel = "models/health_station/health_station.vmdl";
	private TimeUntil _nextHeal = 0;
	private TimeUntil _nextRecharge = 0;

	private const int HEALAMOUNT = 5;
	private const int HEALFREQUENCY = 1; // seconds

	public override void Spawn()
	{
		base.Spawn();

		SetModel( _worldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( StoredHealth < 200f && _nextRecharge && _nextHeal )
		{
			StoredHealth++;
			_nextRecharge = 2f;
		}
	}

	private void HealPlayer( Player player )
	{
		if ( !_nextHeal || StoredHealth <= 0 )
			return;

		float healthNeeded = player.MaxHealth - player.Health;

		if ( healthNeeded <= 0 )
			return;

		float healAmount = Math.Min( StoredHealth, Math.Min( HEALAMOUNT, healthNeeded ) );

		player.SetHealth( player.Health + healAmount );

		StoredHealth -= healAmount;
		_nextHeal = HEALFREQUENCY;
	}

	float IEntityHint.HintDistance => Player.INTERACT_DISTANCE;

	string IEntityHint.TextOnTick => $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to use the Health Station ({StoredHealth} charges)";

	bool IEntityHint.CanHint( Player client )
	{
		return true;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player client )
	{
		return new UI.Hint( (this as IEntityHint).TextOnTick );
	}

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		HealPlayer( player );

		return player.Health < player.MaxHealth;
	}

	bool IUse.IsUsable( Entity user )
	{
		if ( StoredHealth <= 0 || !user.IsAlive() )
			return false;

		if ( user is not Player player || player.Health >= player.MaxHealth )
			return false;

		return true;
	}
}
