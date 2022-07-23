using Sandbox;
using SandboxEditor;

namespace TTT;

[Category( "Equipment" )]
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

		Particles.Create( "particles/cigar/smokepuff", this, "muzzle" );
		_trailParticle = null;
		_trailParticle ??= Particles.Create( "particles/muzzle/barrel_smoke", this, "muzzle" );

		if ( Host.IsServer )
			Owner.TakeDamage( DamageInfo.Generic( 1 ).WithAttacker( Owner ).WithWeapon( this ) );
	}
}
