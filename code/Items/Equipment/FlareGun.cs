using Sandbox;

namespace TTT;

[ClassName( "ttt_equipment_flaregun" )]
[Title( "Flare Gun" )]
public class FlareGun : Weapon
{
	public override string SlotText => AmmoClip.ToString();

	public override void SimulateAnimator( PawnAnimator anim )
	{
		base.SimulateAnimator( anim );

		anim.SetAnimParameter( "holdtype_handedness", 2 );
	}

	protected override void OnHit( TraceResult trace )
	{
		base.OnHit( trace );

		// TODO: Use proper burning once FP implements it.
		var burnDamage = DamageInfo.Generic( 25 )
			.WithAttacker( Owner )
			.WithWeapon( this )
			.WithFlag( DamageFlags.Burn );

		trace.Entity.TakeDamage( burnDamage );

		if ( trace.Entity is Corpse corpse )
			corpse.Delete();
	}
}
