using Sandbox;
using System;

namespace TTT;

[ClassName( "ttt_equipment_newtonlauncher" )]
[Title( "Newton Launcher" )]
public partial class NewtonLauncher : Weapon
{
	[Net, Predicted]
	private float Charge { get; set; }

	[Net, Local, Predicted]
	public bool IsCharging { get; private set; }

	public override string SlotText => $"{(int)Charge}%";

	private const float ChargePerSecond = 50f;
	private const float MaxCharge = 100f;
	private const float MaxForwardForce = 700;
	private const float MinForwardForce = 300;
	private const float MaxUpwardForce = 350;
	private const float MinUpwardForce = 100;

	private float _forwardForce;
	private float _upwardForce;

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		Charge = 0;
		IsCharging = false;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSincePrimaryAttack < Info.PrimaryRate )
			return;

		if ( Input.Down( InputButton.PrimaryAttack ) )
		{
			Charge = Math.Min( MaxCharge, Charge + ChargePerSecond * Time.Delta );

			if ( IsCharging )
				return;

			ChargeStart();
			IsCharging = true;
		}
		else if ( Input.Released( InputButton.PrimaryAttack ) )
		{
			ChargeFinished();
			IsCharging = false;

			using ( LagCompensation() )
			{
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}
	}

	protected override void AttackPrimary()
	{
		ShootEffects();
		PlaySound( Info.FireSound );

		_forwardForce = (Charge / 100f * MinForwardForce) - MinForwardForce + MaxForwardForce;
		_upwardForce = (Charge / 100f * MinUpwardForce) - MinUpwardForce + MaxUpwardForce;

		ShootBullet( Info.Spread, _forwardForce / 100f, Info.Damage, 3.0f, Info.BulletsPerFire );

		Charge = 0;
	}

	protected override void OnHit( TraceResult trace )
	{
		base.OnHit( trace );

		if ( trace.Entity is not Player player )
			return;

		player.GroundEntity = null;

		var pushVel = trace.Direction * _forwardForce;
		pushVel.z = Math.Max( pushVel.z, _upwardForce );

		player.Velocity += pushVel;
	}

	[ClientRpc]
	protected void ChargeStart()
	{
		ViewModelEntity?.SetAnimParameter( "charge", true );
	}

	[ClientRpc]
	protected void ChargeFinished()
	{
		ViewModelEntity?.SetAnimParameter( "charge_finished", true );
	}
}
