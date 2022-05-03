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
	public float Charge { get; set; } = MAX_CHARGE;

	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	[Net, Local]
	public int SelectedId { get; set; }

	public override string SlotText => $"{(int)Charge}%";

	private const float MAX_CHARGE = 100f;
	private const float CHARGE_PER_SECOND = 1f; // TODO: Find proper calculate rate.
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
		if ( selectedDNA == null )
			return;

		var dist = Owner.Position.Distance( selectedDNA.Target ).SourceUnitsToMeters();
		Charge = Math.Max( 0, Charge - Math.Max( 4, dist / 2.16f ) );
		UpdateMarker( selectedDNA.Target );
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
		// TODO: We shouldn't fetch DNA from an armed c4? 

		var samples = trace.Entity.Components.GetAll<DNA>();
		if ( !samples.Any() )
			return;

		foreach ( var dna in samples )
		{
			if ( !dna.TimeUntilDecayed )
				DNACollected.Add( dna ); // TODO: Display a message of how many we fetch, or if it decayed.
			dna.Enabled = false;
		}
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
	public string SourceName { get; private set; }

	public Vector3 Target => TargetPlayer.IsAlive() ? TargetPlayer.Position : TargetPlayer.Corpse.Position;
	public Player TargetPlayer { get; private set; }
	public TimeUntil TimeUntilDecayed { get; private set; }

	public DNA() { }

	public DNA( Player player )
	{
		TargetPlayer = player;
	}

	protected override void OnActivate()
	{
		if ( Host.IsClient )
			return;

		Id = internalId++;

		switch ( Entity )
		{
			case Corpse corpse:
			{
				SourceName = $"{corpse.PlayerName}'s corpse";
				TimeUntilDecayed = (float)Math.Pow( 0.74803 * corpse.DistanceKilledFrom, 2 ) + 100;
			}
			break;
			default:
			{
				SourceName = Entity.ClassInfo.Title;
				TimeUntilDecayed = float.MaxValue; // Never should decay.
			}
			break;
		}
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		internalId = Rand.Int( 0, 500 );
	}
}
