using System;
using System.ComponentModel.Design;
using Sandbox;
using TTT.UI;

namespace TTT;

public partial class PropPossession : EntityComponent<Player>
{
	[Net] public Prop Prop { get; set; }
	[Net, Local] public int Punches { get; set; }

	// Clientside:
	private PossessionNameplate _nameplate;
	private PossessionInfo _info;

	// Serverside:
	private Player _player; // Entity property is cleared before OnDeactivate() called => need to store separately
	private TimeUntil _timeUntilRecharge = 0;
	private TimeUntil _timeUntilNextPunchAllowed = 0;

	public PropPossession() { }

	public PropPossession( Prop prop ) { Prop = prop; }

	protected override void OnActivate()
	{
		_player = Entity;

		if ( Host.IsServer )
		{
			_player.Camera = new FollowEntityCamera( Prop );
			Prop.Owner = Entity;
			_timeUntilRecharge = Game.PropPossessionRechargeTime;
		}
		else
		{
			_nameplate = new PossessionNameplate( Entity, Prop );
			_info = new PossessionInfo( this );
		}
	}

	protected override void OnDeactivate()
	{
		if ( Host.IsServer )
		{
			if ( _player.IsValid() )
				_player.Camera = new FreeSpectateCamera();

			if ( Prop.IsValid() )
				Prop.Owner = null;
		}
		else
		{
			_nameplate?.Delete();
			_info?.Delete();
		}

		_player = null;
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Duck ) )
		{
			CancelPossession();
			return;
		}

		float forward = input.Pressed( InputButton.Forward ) ? 1f : (input.Pressed( InputButton.Back ) ? -1f : 0f);
		float left = input.Pressed( InputButton.Left ) ? 1f : (input.Pressed( InputButton.Right ) ? 1f : 0f);
		bool up = input.Pressed( InputButton.Jump );
		Rotation rotation = Rotation.From( input.ViewAngles );

		if ( forward + left != 0f || up )
			MoveProp( forward, left, up, rotation );
	}

	[ConCmd.Server]
	private static void CancelPossession()
	{
		(ConsoleSystem.Caller.Pawn as Player).Components.RemoveAny<PropPossession>();
	}

	[ConCmd.Server]
	private static void MoveProp( float forward, float left, bool up, Rotation rotation )
	{
		(ConsoleSystem.Caller.Pawn as Player).Components.Get<PropPossession>()
			.HandlePropMovement( forward, left, up, rotation );
	}

	private void HandlePropMovement( float forward, float left, bool up, Rotation rotation )
	{
		var b = Prop.PhysicsBody;

		// reference:
		// https://github.com/Facepunch/garrysmod/blob/master/garrysmod/gamemodes/terrortown/gamemode/propspec.lua#L111

		if ( Punches <= 0 )
			return;

		if ( forward + left == 0f && !up )
			return; // no movement

		if ( !_timeUntilNextPunchAllowed ) { return; }

		if ( Prop is IGrabbable grabbable && grabbable.IsHolding )
			return; // can't move if prop is currently being held by a player

		var mass = Math.Min( 150f, b.Mass );
		var force = 110f * 100f;
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

	[Event.Tick.Server]
	private void RechargePossessionPunches()
	{
		if ( _timeUntilRecharge )
		{
			Punches = Math.Min( Punches + 1, Game.PropPossessionMaxPunches );
			_timeUntilRecharge = Game.PropPossessionRechargeTime;
		}
	}
}
