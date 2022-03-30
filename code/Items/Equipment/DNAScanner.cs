using System.Collections.Generic;
using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public partial class DNAScanner : Carriable
{
	[Net, Local]
	public IList<DNA> DNACollected { get; set; }

	[Net, Local]
	public DNA SelectedSample { get; set; }

	[Net, Local]
	public float Charge { get; set; } = 0;

	private const float MAX_CHARGE = 100f;
	private const float RECHARGE_AMOUNT = 0.5f;

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( !Input.Pressed( InputButton.Attack2 ) )
		{

		}

		if ( !Input.Pressed( InputButton.Attack1 ) )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.USE_DISTANCE )
			.Ignore( this )
			.Ignore( Owner )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() )
			return;

		// If body, check if its identified, otherwise prompt the user a message.

		var DNA = trace.Entity.Components.Get<DNA>();
		if ( DNA != null )
		{
			// trace.Entity.Components.Remove( DNA );

			if ( DNA.IsDecayed )
			{
				Log.Info( "DNA is decayed, we need to remove it." );
				return;
			}

			Log.Info( "DNA fetched successfully!" );
			DNACollected.Add( DNA );
		}
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( Owner is null )
			return;

		if ( Charge <= MAX_CHARGE )
			Charge += RECHARGE_AMOUNT;

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
			UI.RoleMenu.Instance.RemoveTab( RawStrings.DNATab );
	}

	private void Scan()
	{
		Charge -= 50;
	}
}

public class DNA : EntityComponent<Entity>
{
	public enum Type
	{
		Carriable,
		Corpse,
		Ammo // Might not be possible let's double check.
	}

	public Type DNAType { get; set; }
	private TimeSince _timeSinceCreated; // We will use this to figure out when the DNA decays completely. Lets use TimeUntil instead...
	private float _timeToDecay = 60000; // This value is calculated based on the entity, the time to completely decay.

	public bool IsDecayed => _timeSinceCreated > _timeToDecay;

	protected override void OnActivate()
	{
		base.OnActivate();

		_timeSinceCreated = 0;

		switch ( Entity )
		{
			case Carriable:
				DNAType = Type.Carriable;
				break;
			case Corpse:
				DNAType = Type.Corpse;
				break;
			case Ammo:
				DNAType = Type.Ammo;
				break;
		}
	}
}
