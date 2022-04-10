using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/c4/c4.vmdl" )]
[Library( "ttt_entity_c4", Title = "C4" )]
public partial class C4Entity : Prop, IEntityHint
{
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

	[Net, Local]
	public TimeUntil TimeUntilExplode { get; private set; }

	private RealTimeUntil _nextBeepTime = 0f;
	private float _totalSeconds = 0f;
	private readonly List<int> _safeWireNumbers = new();

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public void Arm( Player player, int timer )
	{
		var possibleSafeWires = Enumerable.Range( 1, Wires.Count ).ToList();
		possibleSafeWires.Shuffle();

		var safeWireCount = Wires.Count - GetBadWireCount( timer );
		for ( int i = 0; i < safeWireCount; ++i )
			_safeWireNumbers.Add( possibleSafeWires[i] );

		_totalSeconds = timer;
		TimeUntilExplode = timer;
		IsArmed = true;

		player.Components.Add( new C4Note( _safeWireNumbers.First() ) );
		PlaySound( RawStrings.C4Plant );

		CloseC4ArmMenu();
		if ( player.Team == Team.Traitors )
			SendC4Marker( player.Team.ToAliveClients(), this );
	}

	public static int GetBadWireCount( int timer )
	{
		return Math.Min( (int)MathF.Ceiling( timer / MinTime ), Wires.Count - 1 );
	}

	public void AttemptDefuse( int wire )
	{
		if ( !IsArmed )
			return;

		if ( !_safeWireNumbers.Contains( wire ) )
		{
			Explode( true );
			return;
		}

		IsArmed = false;
		_safeWireNumbers.Clear();
	}

	private void Explode( bool defusalDetonation = false )
	{
		float radius = 750;

		if ( defusalDetonation )
			radius /= 2.5f;

		Explosion( radius );
		PlaySound( RawStrings.C4Explode );
		Delete();
	}

	private void Explosion( float radius )
	{
		// We should probably just use FindInSphere here...
		// Just replicating old TTT code for now.
		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( !player.IsAlive() || player.IsSpectator )
				continue;

			var diff = player.Position - Position;
			float dist = Vector3.DistanceBetween( Position, player.Position );

			if ( dist > radius )
				continue;

			// TODO: Better way to calculate falloff.
			dist = Math.Max( 0, dist - 490 );
			float damage = 125 - dist / 21 * 12;

			var damageInfo = DamageInfo.Explosion( Position, diff.Normal * damage, damage )
				.WithAttacker( Owner )
				.WithWeapon( this );

			player.TakeDamage( damageInfo );
		}
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
			PlaySound( RawStrings.C4Beep );
			_nextBeepTime = _totalSeconds / 45;
		}

		if ( TimeUntilExplode )
			Explode();
	}

	[ServerCmd]
	public static void ArmC4( int networkIdent, int time )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.Arm( player, time );
	}

	[ServerCmd]
	public static void Defuse( int wire, int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		c4.AttemptDefuse( wire );
	}

	[ServerCmd]
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

	[ServerCmd]
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
		if ( !IsLocalPawn )
			return;

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
