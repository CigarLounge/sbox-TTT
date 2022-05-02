using System;
using System.Collections.Generic;
using Sandbox;
using TTT.UI;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public partial class DNAScanner : Carriable
{
	[Net, Local]
	public IList<DNA> DNACollected { get; set; }

	[Net, Local]
	public float Charge { get; set; } = MAX_CHARGE;

	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	[Net, Local]
	public int SelectedId { get; set; }

	public override string SlotText => $"{(int)Charge}%";

	private const float MAX_CHARGE = 100f;
	private const float CHARGE_PER_SECOND = 5f; // TODO: Find proper calculate rate.
	private DNAMarker _dnaMarker;

	public override void Simulate( Client client )
	{
		if ( Input.Pressed( InputButton.Attack1 ) )
			FetchDNA();

		if ( Input.Pressed( InputButton.Attack2 ) )
			Scan();
	}

	private void Scan()
	{
		if ( Charge < MAX_CHARGE )
			return;

		var selectedDNA = FindSelectedDNA( SelectedId );
		if ( selectedDNA == null || selectedDNA.Target == null )
			return;

		var dist = Owner.Position.Distance( selectedDNA.Target.Position ).SourceUnitsToMeters();
		Charge = Math.Max( 0, Charge - Math.Max( 4, dist / 2.16f ) );
		UpdateMarker( selectedDNA.Target.Position );
	}

	private void FetchDNA()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( this )
			.Ignore( Owner )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !trace.Entity.IsValid() )
			return;

		// TODO: We apparently shouldn't allow DNA fetching on unconfirmed bodies.

		var DNA = trace.Entity.Components.Get<DNA>();
		if ( DNA == null )
			return; // TODO Display InfoFeed message.

		trace.Entity.Components.Remove( DNA );

		if ( DNA.TimeUntilDecayed )
			return; // TODO Display InfoFeed message.

		DNACollected.Add( DNA );
	}

	private DNA FindSelectedDNA( int id )
	{
		foreach ( var sample in DNACollected )
			if ( sample.Id == id )
				return sample;

		return null;
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( Owner is null )
			return;

		Charge = Math.Min( Charge + CHARGE_PER_SECOND * Time.Delta, MAX_CHARGE );
		Scan();
	}

	public override void CreateHudElements()
	{
		base.CreateHudElements();

		RoleMenu.Instance?.AddDNATab();
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		if ( !dropped || IsServer )
			return;

		RoleMenu.Instance?.RemoveTab( RoleMenu.DNATab );
		_dnaMarker?.Delete( true );
	}

	[ClientRpc]
	private void UpdateMarker( Vector3 pos )
	{
		PlaySound( "dna-beep" );
		_dnaMarker?.Delete();
		_dnaMarker = new DNAMarker( pos );
	}
}

public partial class DNA : EntityComponent<Entity>
{
	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	[Net]
	public int Id { get; private set; }
	private static int internalId = Rand.Int( 0, 500 );

	[Net]
	public float TimeCollected { get; private set; }

	public enum SourceType
	{
		Corpse
	}

	public SourceType Source { get; private set; }
	public TimeUntil TimeUntilDecayed { get; private set; }
	public Entity Target { get; private set; }

	protected override void OnActivate()
	{
		if ( Host.IsClient )
			return;

		Id = internalId++;

		if ( Game.Current.State is InProgress inProgress )
			TimeCollected = inProgress.FakeTime;

		switch ( Entity )
		{
			case Corpse corpse:
			{
				Source = SourceType.Corpse;
				TimeUntilDecayed = (float)Math.Pow( 0.74803 * corpse.DistanceKilledFrom, 2 ) + 100;
				Target = corpse.KillInfo.Attacker;
			}
			break;
		}
	}
}
