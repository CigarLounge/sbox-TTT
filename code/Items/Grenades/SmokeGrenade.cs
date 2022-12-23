using Editor;
using Sandbox;

namespace TTT;

[Category( "Grenades" )]
[ClassName( "ttt_grenade_smoke" )]
[EditorModel( "models/weapons/w_smoke.vmdl" )]
[HammerEntity]
[Title( "Smoke Grenade" )]
public class SmokeGrenade : Grenade
{
	private const string ExplodeSound = "smoke_explode-1";
	private const string Particle = "particles/smokegrenade/explode.vpcf";

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
