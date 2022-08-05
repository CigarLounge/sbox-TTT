using System;
using System.Linq;
using Sandbox;
using TTT.UI;

namespace TTT;

public partial class PropPossession : EntityComponent<Player>
{
	public const int MaxPunches = 8;
	public const float RechargeTime = 1f;

	[Net]
	public Prop Prop { get; set; }

	[Net, Local]
	public int Punches { get; set; }

	private Player _player; // `Entity` property is cleared before OnDeactivate() called => need to store separately
	private TimeUntil _timeUntilRecharge = 0;
	private TimeUntil _timeUntilNextPunchAllowed = 0;

	// Clientside
	private PossessionNameplate _nameplate;
	private PunchMeter _meter;

	public PropPossession() { }

	public PropPossession( Prop prop ) => Prop = prop;

	protected override void OnActivate()
	{
		_player = Entity;

		if ( Host.IsServer )
		{
			_player.Camera = new FollowEntityCamera( Prop );
			Prop.Owner = Entity;
			_timeUntilRecharge = RechargeTime;
		}
		else
		{
			if ( Local.Pawn is Player player && !player.IsAlive() )
				_nameplate = new PossessionNameplate( Entity, Prop );

			if ( Entity.IsLocalPawn )
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

		_player = null;
	}

	[GameEvent.Player.StatusChanged]
	private static void OnStatusChanged( Player player, PlayerStatus oldStatus )
	{
		if ( player.IsAlive() )
		{
			if ( Host.IsServer )
			{
				// player is not possessing anything anymore since they have come alive
				player.Components.RemoveAny<PropPossession>();
			}

			if ( Host.IsClient && player.IsLocalPawn )
			{
				// local player has come alive, must loose all nameplates
				foreach ( var entity in Sandbox.Entity.All.Where( e => e.Components.Get<PropPossession>() is not null ) )
				{
					entity.Components.Get<PropPossession>()._nameplate?.Delete();
				}
			}
		}
		else
		{
			// Test
			if ( player.IsLocalPawn )
			{
				// local player has died => show nameplates
				foreach ( var entity in Sandbox.Entity.All.Where( e => e.Components.Get<PropPossession>() is not null ) )
				{
					var possession = entity.Components.Get<PropPossession>();
					possession._nameplate?.Delete();
					possession._nameplate = new PossessionNameplate( possession._player, possession.Prop );
				}
			}
		}
	}

	[Event.Tick.Server]
	private static void RechargePunchesAndCheckProp()
	{
		foreach ( var entity in Sandbox.Entity.All.Where( e => e.Components.Get<PropPossession>() is not null ) )
		{
			var possession = entity.Components.Get<PropPossession>();

			if ( !possession.Prop.IsValid() )
			{
				entity.Components.RemoveAny<PropPossession>();
				return;
			}

			if ( possession._timeUntilRecharge )
			{
				possession.Punches = Math.Min( possession.Punches + 1, MaxPunches );
				possession._timeUntilRecharge = RechargeTime;
			}
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

		// Cancel jump button so that spectator camera is not changed
		input.SetButton( InputButton.Jump, false );
	}

	[ConCmd.Server]
	public static void BeginPossession( int propNetworkIdent )
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
		ConsoleSystem.Caller.Pawn.Components.Get<PropPossession>().HandlePropMovement( forward, left, up, rotation );
	}

	private void HandlePropMovement( float forward, float left, bool up, Rotation rotation )
	{
		var b = Prop.PhysicsBody;

		if ( Punches <= 0 )
			return;

		if ( Math.Abs( forward ) > 1f || Math.Abs( left ) > 1f )
			return; // illegal values for forward/left

		if ( !_timeUntilNextPunchAllowed ) { return; }

		if ( Prop is IGrabbable grabbable && grabbable.IsHolding )
			return; // can't move if prop is currently being held by a player

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
