using Sandbox;
using SandboxEditor;

namespace TTT;

[EditorModel( "models/weapons/w_smoke.vmdl" )]
[Library( "ttt_grenade_smoke", Title = "Smoke Grenade" ), HammerEntity]
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
