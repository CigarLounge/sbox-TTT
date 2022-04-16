using Sandbox;

namespace TTT;

[Library( "ttt_grenade_smoke", Title = "Smoke Grenade" )]
public class SmokeGrenade : Grenade
{
	protected override void OnExplode()
	{
		base.OnExplode();

		Particles.Create( "particles/smoke_explode.vpcf", Position );
	}
}
