using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_flaregun" )]
[Title( "Flare Gun" )]
public class FlareGun : Weapon
{
	public override string SlotText => AmmoClip.ToString();

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		base.SimulateAnimator( anim );
		anim.Handedness = CitizenAnimationHelper.Hand.Right;
	}

	protected override void OnHit( TraceResult trace )
	{
		base.OnHit( trace );

		// TODO: Use proper burning once FP implements it.
		var burnDamage = DamageInfo.Generic( 25 )
			.WithAttacker( Owner )
			.WithTag( Strings.Tags.Burn )
			.WithWeapon( this );

		trace.Entity.TakeDamage( burnDamage );

		if ( trace.Entity is Corpse )
			trace.Entity.Delete();
	}
}
