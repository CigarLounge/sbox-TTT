using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_dnascanner", Title = "DNA Scanner" )]
public class DNAScanner : Carriable
{
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

		var DNA = trace.Entity.Components.Get<DNA>();
		if ( DNA != null )
		{
			Log.Info( "DNA was found!" );
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

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Entity is Carriable )
			DNAType = Type.Carriable;
		else if ( Entity is Corpse )
			DNAType = Type.Corpse;
		else if ( Entity is Ammo )
			DNAType = Type.Ammo;
	}
}
