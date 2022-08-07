﻿using System;
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
				_nameplates.Add( new PossessionNameplate( _player, Prop ) );

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
			for ( var i = _nameplates.Count - 1; i >= 0; i-- )
			{
				var nameplate = _nameplates[i];
				if ( nameplate.Player != _player )
					continue;

				nameplate.Delete( true );
				_nameplates.Remove( nameplate );
			}
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
		if ( Host.IsClient && player.IsLocalPawn )
		{
			foreach ( var ent in Sandbox.Entity.All )
			{
				if ( ent is not Player otherPlayer )
					continue;

				if ( otherPlayer.Components.TryGet( out PropPossession possessionComponent ) )
					_nameplates.Add( new PossessionNameplate( possessionComponent._player, possessionComponent.Prop ) );
			}
		}
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !Prop.IsValid() )
			_player.Components.RemoveAny<PropPossession>();

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

		var vertical = input.Pressed( InputButton.Forward ) ? 1f : (input.Pressed( InputButton.Back ) ? -1f : 0f);
		var horizontal = input.Pressed( InputButton.Left ) ? 1f : (input.Pressed( InputButton.Right ) ? -1f : 0f);
		var jump = input.Pressed( InputButton.Jump );
		var rotation = Rotation.From( input.ViewAngles );

		if ( vertical != 0f || horizontal != 0f || jump )
			MoveProp( vertical, horizontal, jump, rotation );

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
	private static void MoveProp( float vertical, float horizontal, bool jump, Rotation rotation )
	{
		ConsoleSystem.Caller.Pawn.Components.Get<PropPossession>()?.HandlePropMovement( vertical, horizontal, jump, rotation );
	}

	private void HandlePropMovement( float vertical, float horizontal, bool jump, Rotation rotation )
	{
		var b = Prop.PhysicsBody;

		if ( Punches <= 0 )
			return;

		if ( !_timeUntilNextPunchAllowed )
			return;

		var mass = Math.Min( 150f, b.Mass );
		var force = 110f * 75f;
		var aim = Vector3.Forward * rotation;
		var mf = mass * force;

		_timeUntilNextPunchAllowed = 0.15f;

		if ( jump )
		{
			b.ApplyForceAt( b.MassCenter, new Vector3( 0, 0, mf ) );
			_timeUntilNextPunchAllowed = 0.2f;
		}
		else if ( vertical != 0f )
		{
			b.ApplyForceAt( b.MassCenter, vertical * aim * mf );
		}
		else if ( horizontal != 0f )
		{
			b.ApplyAngularImpulse( new Vector3( 0, 0, horizontal * 200f * 10f ) );
			b.ApplyForceAt( b.MassCenter, new Vector3( 0, 0, mf / 3f ) );
		}

		Punches = Math.Max( Punches - 1, 0 );
	}
}
