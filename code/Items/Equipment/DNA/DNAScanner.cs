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

	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	// Unable to network "DNA" by itself due to s&box...
	[Net, Local]
	public int? SelectedId { get; set; }

	[Net, Local]
	public bool AutoScan { get; set; } = false;

	[Net, Local]
	private float Charge { get; set; } = MAX_CHARGE;

	public override string SlotText => $"{(int)Charge}%";
	public bool IsCharged => Charge < MAX_CHARGE;

	private const float MAX_CHARGE = 100f;
	private const float CHARGE_PER_SECOND = 2.2f;
	private DNAMarker _dnaMarker;

	public override void Simulate( Client client )
	{
		if ( IsClient && SelectedId == null )
			_dnaMarker?.Delete();

		if ( Input.Pressed( InputButton.Attack1 ) )
			FetchDNA();

		if ( Input.Pressed( InputButton.Attack2 ) )
			Scan();
	}

	public void Scan()
	{
		if ( IsCharged || IsClient )
			return;

		var selectedDNA = FindSelectedDNA( SelectedId );
		if ( selectedDNA == null )
			return;

		var target = selectedDNA.GetTarget();
		if ( !target.IsValid() )
		{
			SelectedId = null;
			DNACollected.Remove( selectedDNA );
			UI.InfoFeed.DisplayEntry( To.Single( Owner ), "DNA not detected in area." );
			return;
		}

		var dist = Owner.Position.Distance( target.Position );
		Charge = Math.Max( 0, Charge - Math.Max( 4, dist / 25 ) );
		UpdateMarker( To.Single( Owner ), target.Position );
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

		var totalCollected = 0;
		foreach ( var dna in samples )
		{
			if ( dna.TimeUntilDecayed )
			{
				dna.Enabled = false;
				continue;
			}

			if ( !DNACollected.Contains( dna ) )
			{
				DNACollected.Add( dna );
				totalCollected += 1;
			}
		}

		if ( totalCollected > 0 )
			UI.InfoFeed.DisplayEntry( To.Single( Owner ), $"Collected {totalCollected} new DNA sample(s)." );
	}

	private DNA FindSelectedDNA( int? id )
	{
		if ( id == null )
			return null;

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

		if ( AutoScan )
			Scan();
	}

	public override void OnClientCarryStart( Entity carrier )
	{
		RoleMenu.Instance?.AddDNATab();
	}

	public override void OnClientCarryDrop( Entity carrier )
	{
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

	public Entity GetTarget()
	{
		if ( !TargetPlayer.IsValid() )
			return null;

		var decoyComponent = TargetPlayer.Components.Get<DecoyComponent>();
		if ( decoyComponent != null && decoyComponent.Decoy.IsValid() )
			return decoyComponent.Decoy;

		return TargetPlayer.IsAlive() ? TargetPlayer : TargetPlayer.Corpse;
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		internalId = Rand.Int( 0, 500 );
	}
}
