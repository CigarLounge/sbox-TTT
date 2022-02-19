using System;

using Sandbox;

namespace TTT;

[Library( "ttt_equipment_healthstation_ent", Title = "Health Station" )]
public partial class HealthStationEntity : Prop, IEntityHint
{
	[Net]
	public float StoredHealth { get; set; } = 200f; // This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

	private const string _worldModel = "models/health_station/health_station.vmdl";
	private RealTimeUntil NextHeal = 0;

	private const int HEALAMOUNT = 1;
	private const int HEALFREQUENCY = 1; // seconds
	private const int DELAYIFFAILED = 2; // Multiplied by HealFrequency if HealthPlayer returns false

	public override void Spawn()
	{
		base.Spawn();

		SetModel( _worldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	private bool HealPlayer( Player player )
	{
		float healthNeeded = player.MaxHealth - player.Health;

		if ( StoredHealth > 0 && healthNeeded > 0 )
		{
			float healAmount = Math.Min( HEALAMOUNT, healthNeeded );

			player.SetHealth( player.Health + healAmount );

			StoredHealth -= healAmount;

			return true;
		}

		return false;
	}

	public float HintDistance => Player.INTERACT_DISTANCE;

	public string TextOnTick => $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to use the Health Station ({StoredHealth} charges)";

	public bool CanHint( Player client )
	{
		return true;
	}

	public UI.EntityHintPanel DisplayHint( Player client )
	{
		return new UI.Hint( TextOnTick );
	}

	public void Tick( Player player )
	{
		if ( IsClient )
		{
			return;
		}

		if ( player.LifeState != LifeState.Alive )
		{
			return;
		}

		using ( Prediction.Off() )
		{
			if ( Input.Down( InputButton.Use ) )
			{
				if ( player.Health < player.MaxHealth && NextHeal <= 0 )
				{
					NextHeal = HealPlayer( player ) ? HEALFREQUENCY : HEALFREQUENCY * DELAYIFFAILED;
				}
			}
		}
	}
}
