using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

[ClassName( "ttt_entity_c4" )]
[EditorModel( "models/c4/c4.vmdl" )]
[Title( "C4" )]
public partial class C4Entity : Prop, IEntityHint
{
	public const string BeepSound = "c4_beep-1";
	public const string PlantSound = "c4_plant-1";
	public const string DefuseSound = "c4_defuse-1";
	public const string ExplodeSound = "c4_explode-2";
	public const float MaxTime = 600;
	public const float MinTime = 45;

	public static readonly List<Color> Wires = new()
	{
		Color.Red,
		Color.Yellow,
		Color.Blue,
		Color.White,
		Color.Green,
		Color.FromBytes( 255, 160, 50, 255 ) // Brown
	};

	private static readonly Model WorldModel = Model.Load( "models/c4/c4.vmdl" );

	[Net]
	public bool IsArmed { get; private set; }

	[Net]
	public TimeUntil TimeUntilExplode { get; private set; }

	private RealTimeUntil _nextBeepTime = 0f;
	private float _totalSeconds = 0f;
	private UI.C4Timer _c4Timer;
	private readonly List<int> _safeWireNumbers = new();

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		_c4Timer = new( this );
	}

	public void Arm( Player player, int timer )
	{
		// Incase another player sends in a request before their UI is updated.
		if ( IsArmed )
			return;

		var possibleSafeWires = Enumerable.Range( 1, Wires.Count ).ToList();
		possibleSafeWires.Shuffle();

		var safeWireCount = Wires.Count - GetBadWireCount( timer );
		for ( var i = 0; i < safeWireCount; ++i )
			_safeWireNumbers.Add( possibleSafeWires[i] );

		_totalSeconds = timer;
		TimeUntilExplode = timer;
		IsArmed = true;

		player.Components.Add( new C4Note( _safeWireNumbers.First() ) );
		PlaySound( PlantSound );

		CloseC4ArmMenu();
		if ( player.Team == Team.Traitors )
			SendC4Marker( player.Team.ToAliveClients(), this );
	}

	public static int GetBadWireCount( int timer )
	{
		return Math.Min( (int)MathF.Ceiling( timer / MinTime ), Wires.Count - 1 );
	}

	public void AttemptDefuse( Player defuser, int wire )
	{
		if ( !IsArmed )
			return;

		if ( defuser != Owner && !_safeWireNumbers.Contains( wire ) )
			Explode( true );
		else
			Defuse();
	}

	public void Defuse()
	{
		PlaySound( DefuseSound );
		IsArmed = false;
		_safeWireNumbers.Clear();
	}

	private void Explode( bool defusalDetonation = false )
	{
		float radius = 1500;

		if ( defusalDetonation )
			radius /= 1.5f;

		Explosion( radius );
		Sound.FromWorld( ExplodeSound, Position );
		Delete();
	}

	private void Explosion( float radius )
	{
		foreach ( var entity in FindInSphere( Position, radius ) )
		{
			if ( entity is not Player player || !player.IsAlive() )
				continue;

			var diff = player.Position - Position;
			var dist = Vector3.DistanceBetween( Position, player.Position );
			var damage = 225 - (0.1f * dist);

			if ( damage <= 0 )
				continue;

			var damageInfo = DamageInfo.Explosion( Position, diff.Normal * damage, damage )
				.WithAttacker( Owner )
				.WithWeapon( this );

			player.TakeDamage( damageInfo );
		}
	}

	protected override void OnDestroy()
	{
		_c4Timer?.Delete( true );

		base.OnDestroy();
	}

	void IEntityHint.Tick( Player player )
	{
		if ( !player.IsLocalPawn || !player.IsAlive() || !Input.Down( InputButton.Use ) )
		{
			UI.FullScreenHintMenu.Instance?.Close();
			return;
		}

		if ( UI.FullScreenHintMenu.Instance.IsOpen )
			return;

		if ( IsArmed )
			UI.FullScreenHintMenu.Instance?.Open( new UI.C4DefuseMenu( this ) );
		else
			UI.FullScreenHintMenu.Instance?.Open( new UI.C4ArmMenu( this ) );
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.C4Hint( this );

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( !IsArmed )
			return;

		if ( _nextBeepTime )
		{
			PlaySound( BeepSound );
			_nextBeepTime = _totalSeconds / 45;
		}

		if ( TimeUntilExplode )
			Explode();
	}

	[ConCmd.Server]
	public static void ArmC4( int networkIdent, int time )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.Arm( player, time );
	}

	[ConCmd.Server]
	public static void Defuse( int wire, int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.AttemptDefuse( player, wire );
	}

	[ConCmd.Server]
	public static void Pickup( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		player.Inventory.Add( new C4() );
		c4.Delete();
	}

	[ConCmd.Server]
	public static void DeleteC4( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.Delete();
	}

	[ClientRpc]
	private void CloseC4ArmMenu()
	{
		if ( UI.FullScreenHintMenu.Instance.ActivePanel is UI.C4ArmMenu )
			UI.FullScreenHintMenu.Instance.Close();
	}

	[ClientRpc]
	public static void SendC4Marker( C4Entity c4 )
	{
		UI.WorldPoints.Instance.AddChild( new UI.C4Marker( c4 ) );
	}
}

public class C4Note : EntityComponent
{
	public int SafeWireNumber { get; init; }

	public C4Note() { }

	public C4Note( int wire )
	{
		SafeWireNumber = wire;
	}
}
