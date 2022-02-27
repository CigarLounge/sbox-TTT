using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_knife.vmdl" )]
[Library( "ttt_weapon_knife", Title = "Knife" )]
public partial class Knife : Carriable
{
	[Net, Predicted] public TimeSince TimeSinceStab { get; set; }
	private bool _hasLanded = true;
	private Player _thrower;
	private bool _isThrown = false;

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < Info.DeployTime || TimeSinceStab < 1f )
			return;

		if ( Input.Down( InputButton.Attack1 ) )
		{
			using ( LagCompensation() )
			{
				MeleeAttack( 100f, 100f, 8f );
			}
		}
		else if ( Input.Released( InputButton.Attack2 ) )
		{
			using ( LagCompensation() )
			{
				Throw();
			}
		}
	}

	public override bool CanCarry( Entity carrier )
	{
		return _hasLanded && base.CanCarry( carrier );
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );
		_isThrown = false;
	}

	protected void MeleeAttack( float damage, float range, float radius )
	{
		TimeSinceStab = 0;

		Owner.SetAnimParameter( "b_attack", true );
		SwingEffects();
		PlaySound( RawStrings.SwingSound );

		var endPos = Owner.EyePosition + Owner.EyeRotation.Forward * range;

		var trace = Trace.Ray( Owner.EyePosition, endPos )
			.UseHitboxes( true )
			.Ignore( Owner )
			.Radius( radius )
			.Run();

		if ( !trace.Hit )
			return;

		trace.Surface.DoBulletImpact( trace );

		if ( !IsServer )
			return;

		DamageInfo info = new DamageInfo()
			.WithPosition( trace.EndPosition )
			.UsingTraceResult( trace )
			.WithAttacker( Owner )
			.WithFlag( DamageFlags.Slash )
			.WithWeapon( this );

		info.Damage = damage;

		trace.Entity.TakeDamage( info );

		if ( trace.Entity is Player )
		{
			PlaySound( RawStrings.FleshHit );
			Owner.Inventory.DropActive();
			Delete();
		}
	}

	public override void StartTouch( Entity other )
	{
		base.Touch( other );

		if ( _isThrown )
		{
			if ( other is Player player && player != _thrower )
			{
				var damageInfo = new DamageInfo()
						.WithFlag( DamageFlags.Slash )
						.WithAttacker( Owner )
						.WithWeapon( this );

				Velocity = Vector3.Zero;
				damageInfo.Damage = 100f;
				player.TakeDamage( damageInfo );

				Delete();
			}
			else if ( other.IsWorld )
			{
				PhysicsEnabled = false;
			}
		}

		_hasLanded = true;
	}

	public void Throw()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition ).Ignore( Owner ).Run();
		_thrower = Owner;
		_isThrown = true;

		if ( !IsServer )
		{
			Owner.Inventory.Active = null;
			return;
		}

		var owner = Owner;
		Owner.Inventory.DropActive();
		Rotation = owner.EyeRotation;
		Position = trace.EndPosition;
		Velocity = owner.EyeRotation.Forward * 1000f;
		_hasLanded = false;
	}

	[ClientRpc]
	protected void SwingEffects()
	{
		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}
