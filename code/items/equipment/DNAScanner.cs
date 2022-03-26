using System.Collections.Generic;
using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public partial class DNAScanner : Carriable
{
	[Net, Local]
	public IList<DNA> DNACollected { get; set; }

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

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
			trace.Entity.Components.Remove( DNA );

			if ( DNA.IsDecayed )
			{
				Log.Info( "DNA is decayed, we need to remove it." );
				return;
			}

			Log.Info( "DNA fetched successfully!" );
			DNACollected.Add( DNA );
		}
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( Host.IsClient )
			UI.RoleMenu.Instance.AddDNATab();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( Host.IsClient )
			UI.RoleMenu.Instance.RemoveTab( RawStrings.DNATab );
	}
}

public class DNA : EntityComponent<Entity>
{
	enum Type
	{
		Carriable,
		Corpse,
		Ammo // Might not be possible let's double check.
	}

	private Type DNAType { get; set; }
	private TimeSince _timeSinceCreated; // We will use this to figure out when the DNA decays completely. Lets use TimeUntil instead...
	private float _timeToDecay = 5; // This value is calculated based on the entity, the time to completely decay.

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
