using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_newtonlauncher", Title = "Newton Launcher" )]
public partial class NewtonLauncher : Weapon
{
	[Net, Local, Predicted]
	private int Charge { get; set; }
	private const int MAX_CHARGE = 100;

	public override string SlotText => $"{Charge}%";

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		// While we have no viewmodel let's just show the world model.
		EnableHideInFirstPerson = false;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( Charge < MAX_CHARGE )
			Charge += 1;

		if ( Charge >= MAX_CHARGE )
		{
			using ( LagCompensation() )
			{
				Fire();
			}
		}
	}

	private void Fire()
	{
		Charge = 0;

		var forward = Owner.EyeRotation.Forward;
		forward = forward.Normal;

		foreach ( var trace in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 20000f, 3.0f ) )
		{
			var fullEndPosition = trace.EndPosition + trace.Direction * 3.0f;

			if ( !string.IsNullOrEmpty( Info.TracerParticle ) && trace.Distance > 200 )
			{
				var tracer = Particles.Create( Info.TracerParticle );
				tracer?.SetPosition( 0, trace.StartPosition );
				tracer?.SetPosition( 1, trace.EndPosition );
			}

			if ( !IsServer )
				continue;

			if ( !trace.Entity.IsValid() )
				continue;

			using ( Prediction.Off() )
			{
				trace.Entity.ApplyAbsoluteImpulse( 1000f * fullEndPosition );

				var damageInfo = DamageInfo.FromBullet( trace.EndPosition, forward * 1000f, Info.Damage )
					.UsingTraceResult( trace )
					.WithAttacker( Owner )
					.WithWeapon( this );

				if ( trace.Entity is Player player )
					player.LastDistanceToAttacker = Owner.Position.Distance( player.Position ).SourceUnitsToMeters();

				OnHit( trace.Entity );
				trace.Entity.TakeDamage( damageInfo );
			}
		}
	}
}
