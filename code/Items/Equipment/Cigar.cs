using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_equipment_cigar" )]
[EditorModel( "models/cigar/cigar.vmdl" )]
[HammerEntity]
[Title( "Cigar" )]
public class Cigar : Carriable
{
	private TimeUntil _timeUntilNextSmoke = 0;
	private Particles _trailParticle;

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		_trailParticle?.Destroy( true );
	}

	public override void Simulate( Client client )
	{
		if ( Input.Pressed( InputButton.PrimaryAttack ) && _timeUntilNextSmoke )
			Smoke();
	}

	private void Smoke()
	{
		_timeUntilNextSmoke = 5;

		Particles.Create( "particles/swb/smoke/swb_smokepuff_1", this, "muzzle" );
		_trailParticle = null;
		_trailParticle ??= Particles.Create( "particles/swb/muzzle/barrel_smoke", this, "muzzle" );

		Owner.TakeDamage
		(
			DamageInfo.Generic( 1 )
			.WithAttacker( Owner )
			.WithWeapon( this )
		);
	}
}
