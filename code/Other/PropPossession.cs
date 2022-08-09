using Sandbox;
using Sandbox.UI;
using System;

namespace TTT;

/// <summary>
/// Allows a prop to be controlled by a spectator.
/// </summary>
public partial class PropPossession : EntityComponent<Prop>
{
	[Net, Local]
	public int Punches { get; private set; }

	public const int MaxPunches = 8;
	private const float PunchRechargeTime = 1f;

	private UI.PossessionMeter _meter;
	private UI.PossessionNameplate _nameplate;
	private TimeUntil _timeUntilNextPunch = 0;
	private TimeUntil _timeUntilRecharge = 0;

	// Called from Player.Simulate()
	public void Punch()
	{
		if ( Punches <= 0 )
			return;

		if ( !_timeUntilNextPunch )
			return;

		var physicsBody = Entity.PhysicsBody;

		var mass = Math.Min( 150f, physicsBody.Mass );
		var force = 110f * 75f;
		var aim = Vector3.Forward * Input.Rotation;
		var mf = mass * force;

		_timeUntilNextPunch = 0.15f;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			physicsBody.ApplyForceAt( physicsBody.MassCenter, new Vector3( 0, 0, mf ) );
			_timeUntilNextPunch = 0.2f;
		}
		else if ( Input.Forward != 0f )
		{
			physicsBody.ApplyForceAt( physicsBody.MassCenter, Input.Forward * aim * mf );
		}
		else if ( Input.Left != 0f )
		{
			physicsBody.ApplyAngularImpulse( new Vector3( 0, 0, Input.Left * 200f * 10f ) );
			physicsBody.ApplyForceAt( physicsBody.MassCenter, new Vector3( 0, 0, mf / 3f ) );
		}

		Punches = Math.Max( Punches - 1, 0 );
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsClient && !Local.Pawn.IsAlive() )
			_nameplate = new( Entity );

		if ( Entity.IsLocalPawn )
			_meter = new( this );

	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		_nameplate?.Delete( true );
		_meter?.Delete( true );
	}

	[GameEvent.Player.Killed]
	private void OnPlayerKilled( Player player )
	{
		if ( !player.IsLocalPawn )
			return;

		_nameplate = new( Entity );
	}

	[Event.Tick.Server]
	private void RechargePunches()
	{
		if ( !_timeUntilRecharge )
			return;

		Punches = Math.Min( Punches + 1, MaxPunches );
		_timeUntilRecharge = PunchRechargeTime;
	}
}
