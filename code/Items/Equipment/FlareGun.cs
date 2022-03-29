using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_flaregun", Title = "Flare Gun" )]
public partial class FlareGun : Weapon
{
	public override void SimulateAnimator( PawnAnimator anim )
	{
		base.SimulateAnimator( anim );

		anim.SetAnimParameter( "holdtype_handedness", 2 );
	}

	protected override void OnHit( Entity entity )
	{
		base.OnHit( entity );

		if ( entity is Corpse )
			entity.Delete();
	}
}
