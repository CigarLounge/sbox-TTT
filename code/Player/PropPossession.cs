using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using TTT.UI;

namespace TTT;

public partial class PropPossession : EntityComponent<Player>
{
	public const int MaxPunches = 8;
	private const float RechargeTime = 1f;

	[Net, Local]
	public int Punches { get; private set; }

	[Net]
	private Prop Prop { get; set; }

	private Player _player;
	private TimeUntil _timeUntilRecharge = 0;
	private TimeUntil _timeUntilNextPunchAllowed = 0;

	// Clientside
	private static readonly List<PossessionNameplate> _nameplates = new();
	private PossessionNameplate _nameplate;
	private PunchMeter _meter;

	public PropPossession() { }

	public PropPossession( Prop prop ) => Prop = prop;

	protected override void OnActivate()
	{
		_player = Entity;

		if ( Host.IsServer )
		{
			Prop.Owner = _player;
			_player.Camera = new FollowEntityCamera( Prop );
			_timeUntilRecharge = RechargeTime;
		}
		else
		{
			if ( Local.Pawn is Player player && !player.IsAlive() )
			{
				_nameplate = new PossessionNameplate( _player, Prop );
				_nameplates.Add( _nameplate );
			}

			if ( _player.IsLocalPawn )
				_meter = new PunchMeter( this );
		}
	}

	protected override void OnDeactivate()
	{
		if ( Host.IsServer )
		{
			if ( _player.IsValid() && !_player.IsAlive() )
				_player.Camera = new FreeSpectateCamera();

			if ( Prop.IsValid() )
				Prop.Owner = null;
		}
		else
		{
			_nameplate?.Delete();
			_meter?.Delete();
		}
	}

	[GameEvent.Player.Spawned]
	private static void OnPlayerSpawn( Player player )
	{
		player.Components.RemoveAny<PropPossession>();

		if ( Host.IsClient && player.IsLocalPawn )
		{
			_nameplates.ForEach( ( nameplate ) => nameplate?.Delete() );
			_nameplates.Clear();
		}
	}

	[GameEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		// TODO: Currently active nameplates should be created for the player.
		// Network down some data to them and create em.
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( _timeUntilRecharge )
		{
			Punches = Math.Min( Punches + 1, MaxPunches );
			_timeUntilRecharge = RechargeTime;
		}
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder input )
	{
		if ( !Entity.IsValid() || !Entity.IsLocalPawn )
			return;

		if ( input.Pressed( InputButton.Duck ) )
		{
			CancelPossession();
			return;
		}

		var forward = input.Pressed( InputButton.Forward ) ? 1f : (input.Pressed( InputButton.Back ) ? -1f : 0f);
		var left = input.Pressed( InputButton.Left ) ? 1f : (input.Pressed( InputButton.Right ) ? -1f : 0f);
		var up = input.Pressed( InputButton.Jump );
		var rotation = Rotation.From( input.ViewAngles );

		if ( forward + left != 0f || up )
			MoveProp( forward, left, up, rotation );

		// Ignore any jump inputs since we don't want to change spectating cameras.
		input.SetButton( InputButton.Jump, false );
	}

	[ConCmd.Server]
	public static void Possess( int propNetworkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( player.IsAlive() || player.Components.TryGet<PropPossession>( out _ ) )
			return;

		var target = Sandbox.Entity.FindByIndex( propNetworkIdent );
		if ( !target.IsValid() || target is not Prop prop || prop.PhysicsBody is null || target.Owner is Player )
			return;

		player.Components.Add( new PropPossession( prop ) );
	}

	[ConCmd.Server]
	private static void CancelPossession()
	{
		ConsoleSystem.Caller.Pawn.Components.RemoveAny<PropPossession>();
	}

	[ConCmd.Server]
	private static void MoveProp( float forward, float left, bool up, Rotation rotation )
	{
		ConsoleSystem.Caller.Pawn.Components.Get<PropPossession>()?.HandlePropMovement( forward, left, up, rotation );
	}

	private void HandlePropMovement( float forward, float left, bool up, Rotation rotation )
	{
		var b = Prop.PhysicsBody;

		if ( Punches <= 0 )
			return;

		if ( Math.Abs( forward ) > 1f || Math.Abs( left ) > 1f )
			return; // illegal values for forward/left

		if ( !_timeUntilNextPunchAllowed )
			return;

		var mass = Math.Min( 150f, b.Mass );
		var force = 110f * 75f;
		var aim = Vector3.Forward * rotation;
		var mf = mass * force;

		_timeUntilNextPunchAllowed = 0.15f;

		if ( up )
		{
			b.ApplyForceAt( b.MassCenter, new Vector3( 0, 0, mf ) );
			_timeUntilNextPunchAllowed = 0.2f; // jump is penalised more
		}
		else if ( forward != 0f )
		{
			b.ApplyForceAt( b.MassCenter, forward * aim * mf );
		}
		else if ( left != 0f )
		{
			b.ApplyAngularImpulse( new Vector3( 0, 0, left * 200f * 10f ) );
			b.ApplyForceAt( b.MassCenter, new Vector3( 0, 0, mf / 3f ) );
		}

		Punches = Math.Max( Punches - 1, 0 );
	}
}
