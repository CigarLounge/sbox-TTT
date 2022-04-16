using Sandbox;
using System;

namespace TTT;

[Library( "ttt_grenade_smoke", Title = "Smoke" )]
public class SmokeGrenade : Grenade
{
	protected override void OnExplode()
	{
		base.OnExplode();
	}
}
