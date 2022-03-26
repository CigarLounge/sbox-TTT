using System.Collections.Generic;
using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public partial class DNAScanner : Carriable
{
	[Net]
	public IList<DNA> DNACollected { get; set; }

	public override void Simulate( Client client )
	{
		if ( IsClient )
			Log.Info( DNACollected.Count );

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
			if ( DNA.IsDecayed() )
			{
				Log.Info( "DNA is decayed, we need to remove it." );
				trace.Entity.Components.Remove( DNA );
				return;
			}

			Log.Info( "DNA fetched successfully!" );
			DNACollected.Add( DNA );
		}
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
	private TimeSince _timeSinceCreated; // We will use this to figure out when the DNA decays completely.
	private float _timeToDecay = 5; // This value is calculated based on the entity, the time to completely decay.

	protected override void OnActivate()
	{
		base.OnActivate();

		_timeSinceCreated = 0;

		if ( Entity is Carriable )
			DNAType = Type.Carriable;
		else if ( Entity is Corpse )
			DNAType = Type.Corpse;
		else if ( Entity is Ammo )
			DNAType = Type.Ammo;
	}

	// Maybe we should remove the component from inside here? is that possible?
	// If (IsDecayed())
	//      remove this component here.
	public bool IsDecayed()
	{
		return _timeSinceCreated > _timeToDecay;
	}
}
