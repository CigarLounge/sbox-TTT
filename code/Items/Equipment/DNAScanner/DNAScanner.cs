using System;
using System.Collections.Generic;
using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public partial class DNAScanner : Carriable
{
	[Net, Local]
	public IList<DNA> DNACollected { get; set; }

	// Waiting on https://github.com/Facepunch/sbox-issues/issues/1719
	public DNA SelectedDNA => DNACollected.IsNullOrEmpty() ? null : DNACollected[0];

	public override string SlotText => $"{(int)Charge}%";

	[Net, Local]
	public float Charge { get; set; } = 0;

	private const float MAX_CHARGE = 100f;
	private const float CHARGE_PER_SECOND = 30f;

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
			FetchDNA();

		if ( Input.Pressed( InputButton.Attack2 ) )
			Scan();
	}

	private void Scan()
	{
		if ( SelectedDNA == null )
			return;

		Charge -= 50;
	}

	private void FetchDNA()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( this )
			.Ignore( Owner )
			.EntitiesOnly()
			.Run();

		if ( !trace.Entity.IsValid() )
			return;

		// Unique case where corpse needs to be identified before fetching DNA.
		if ( trace.Entity is Corpse corpse && !corpse.DeadPlayer.IsValid() )
			return;

		var DNA = trace.Entity.Components.Get<DNA>();
		if ( DNA == null )
			return;

		if ( DNA.TimeUntilDecayed )
		{
			// TODO: Display a message in info feed.
			Log.Info( "DNA is decayed, we need to remove it." );
			return;
		}

		DNACollected.Add( DNA );
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( Owner is null )
			return;

		Charge = Math.Min( Charge + CHARGE_PER_SECOND * Time.Delta, MAX_CHARGE );

		if ( Charge == MAX_CHARGE )
			Scan();
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( IsLocalPawn )
			UI.RoleMenu.Instance.AddDNATab();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( IsLocalPawn )
			UI.RoleMenu.Instance.RemoveTab( UI.RoleMenu.DNATab );
	}
}

public class DNA : EntityComponent<Entity>
{
	public enum Type
	{
		Corpse
	}

	public Type DNAType { get; private set; }
	public TimeUntil TimeUntilDecayed { get; private set; }
	public Entity Target { get; private set; }

	protected override void OnActivate()
	{
		switch ( Entity )
		{
			case Corpse corpse:
			{
				DNAType = Type.Corpse;
				TimeUntilDecayed = 900000;
				Target = corpse.KillInfo.Attacker;
			}
			break;
		}
	}
}
