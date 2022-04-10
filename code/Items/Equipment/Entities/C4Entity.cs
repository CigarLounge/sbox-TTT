using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using TTT.UI;

namespace TTT;

[Hammer.EditorModel( "models/c4/c4.vmdl" )]
[Library( "ttt_entity_c4", Title = "C4" )]
public partial class C4Entity : Prop, IEntityHint, IUse
{
	public static readonly List<Color> Wires = new() {
		Color.Magenta,
		Color.Red,
		Color.Blue,
		Color.Yellow,
		Color.Cyan,
		Color.Green
	};

	private static readonly Model WorldModel = Model.Load( "models/c4/c4.vmdl" );

	[Net, Local]
	public bool IsArmed { get; private set; }

	[Net, Local]
	public TimeUntil TimeUntilExplode { get; private set; }

	private readonly List<int> _safeWireNumbers = new();

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 100f;
	}

	public void Arm( Player player, int timer )
	{
		var possibleSafeWires = Enumerable.Range( 1, Wires.Count ).ToList();
		possibleSafeWires.Shuffle();

		var safeWireCount = Wires.Count - GetBadWireCount( timer );
		for ( int i = 0; i < safeWireCount; ++i )
			_safeWireNumbers.Add( possibleSafeWires[i] );

		TimeUntilExplode = timer;
		IsArmed = true;
		CloseC4ArmMenu();

		player.Components.Add( new C4Note( _safeWireNumbers.First() ) );
		SendC4Marker( To.Multiple( Utils.GetAliveClientsWithRole( new TraitorRole() ) ), NetworkIdent );
	}

	public static int GetBadWireCount( int timer )
	{
		return (int)Math.Min( Math.Ceiling( timer / 60.0 ), Wires.Count - 1 );
	}

	public void AttemptDefuse( int wire )
	{
		if ( !_safeWireNumbers.Contains( wire ) )
		{
			Explode();
			return;
		}

		IsArmed = false;
		_safeWireNumbers.Clear();
	}

	private void Explode()
	{
		// TODO: Explode.
		Delete();
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

	bool IUse.OnUse( Entity user )
	{
		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user is Player;
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( !IsArmed )
			return;

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
	public static void SendC4Marker( int networkIdent )
	{
		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not C4Entity c4 )
			return;

		WorldPoints.Instance.AddChild( new C4Marker( c4 ) );
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