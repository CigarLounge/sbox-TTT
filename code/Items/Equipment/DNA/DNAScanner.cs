using System;
using System.Collections.Generic;
using System.Linq;
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
	public float Charge { get; set; } = 0;

	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	[Net, Local]
	public int SelectedId { get; set; }

	public override string SlotText => $"{(int)Charge}%";

	private const float MAX_CHARGE = 100f;
	private const float CHARGE_PER_SECOND = 30f;
	private DNAMarker _dnaMarker;

	public override void Simulate( Client client )
	{
		if ( IsClient )
			return;

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

		Charge -= 50;
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

		var DNA = trace.Entity.Components.Get<DNA>();
		if ( DNA == null )
			return;

		trace.Entity.Components.Remove( DNA );

		if ( DNA.TimeUntilDecayed )
		{
			// TODO: Display a message in info feed.
			return;
		}

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

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

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

	public enum Type
	{
		Corpse
	}

	public Type DNAType { get; private set; }
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
				DNAType = Type.Corpse;
				TimeUntilDecayed = (float)Math.Pow( 0.74803 * corpse.DistanceKilledFrom, 2 ) + 100;
				Target = corpse.KillInfo.Attacker;
			}
			break;
		}
	}
}
