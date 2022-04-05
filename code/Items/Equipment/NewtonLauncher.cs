using Sandbox;
using System;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_newtonlauncher", Title = "Newton Launcher" )]
public partial class NewtonLauncher : Weapon
{
	[Net, Predicted]
	private float Charge { get; set; }

	public override string SlotText => $"{(int)Charge}%";

	private const float MaxCharge = 100f;
	private const float ChargePerSecond = 50f;

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( TimeSincePrimaryAttack < Info.PrimaryRate )
			return;

		if ( Input.Down( InputButton.Attack1 ) )
		{
			Charge = Math.Min( MaxCharge, Charge + ChargePerSecond * Time.Delta );
		}
		else if ( Input.Released( InputButton.Attack1 ) )
		{
			using ( LagCompensation() )
			{
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}
	}

	protected override void AttackPrimary()
	{
		Owner.SetAnimParameter( "b_attack", true );
		ShootEffects();
		PlaySound( Info.FireSound );

		ShootBullet( Info.Spread, 0, Info.Damage, 3.0f, Info.BulletsPerFire );

		Charge = 0;
	}

	protected override void OnHit( TraceResult trace )
	{
		base.OnHit( trace );

		if ( trace.Entity.IsWorld )
			return;

		trace.Entity.GroundEntity = null;
		trace.Entity.ApplyAbsoluteImpulse( 10 * Charge * trace.Direction + 5 * Charge * Vector3.Up );
	}
}
