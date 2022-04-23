using Sandbox;

namespace TTT;

[Library( "ttt_grenade_smoke", Title = "Smoke Grenade" )]
[Hammer.EditorModel( "models/weapons/w_smoke.vmdl" )]
public class SmokeGrenade : Grenade
{
	private const string ExplodeSound = "smoke_explode-1";
	private const string Particle = "particles/smoke_explode.vpcf";

	protected override void OnExplode()
	{
		base.OnExplode();

		Particles.Create( Particle, Position );
		Sound.FromWorld( ExplodeSound, Position );
	}

	static SmokeGrenade()
	{
		Precache.Add( Particle );
	}
}
