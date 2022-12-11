using Editor;
using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_cigar" )]
[EditorModel( "models/cigar/cigar.vmdl" )]
[HammerEntity]
[Title( "Cigar" )]
public class Cigar : Carriable
{
	public override string PrimaryAttackHint => "Smoke";
	private TimeUntil _timeUntilNextSmoke = 0;
	private Particles _trailParticle;

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		_trailParticle?.Destroy( true );
	}

	public override void Simulate( IClient client )
	{
		if ( Input.Pressed( InputButton.PrimaryAttack ) && _timeUntilNextSmoke )
			Smoke();
	}

	private void Smoke()
	{
		_timeUntilNextSmoke = 5;

		Particles.Create( "particles/cigar/smokepuff", this, "muzzle" );
		_trailParticle = null;
		_trailParticle ??= Particles.Create( "particles/muzzle/barrel_smoke", this, "muzzle" );

		if ( Game.IsServer )
			Owner.TakeDamage( DamageInfo.Generic( 1 ).WithAttacker( Owner ).WithWeapon( this ) );
	}
}
