using Sandbox;

namespace TTT;

[Category( "Weapons" )]
[ClassName( "ttt_weapon_knife" )]
[HideInEditor]
[Title( "Knife" )]
public partial class Knife : Carriable
{
	[Net, Local, Predicted]
	public TimeSince TimeSinceStab { get; private set; }

	private const string SwingSound = "knife_swing-1";
	private const string FleshHit = "knife_flesh_hit-1";

	private const float NormalDamage = 50.0f;
	private const float BackstabDamage = 100.0f;

	private bool _isThrown = false;
	private Player _thrower;
	private Vector3 _thrownFrom;
	private Rotation _throwRotation = Rotation.From( new Angles( 90, 0, 0 ) );
	private float _gravityModifier;

	private static bool CanBackstab( Vector3 attackerPos, Entity target )
	{
		// Get delta between entity positions (as Vector2, so without Z)
		var delta = new Vector2(
			target.Position.x - attackerPos.x,
			target.Position.y - attackerPos.y );

		// Is the attacker behind the target? (position wise)		
		// +1.0 == directly behind, -1.0 == directly in front, so
		// >0.0 means the attacker is at the back half of the target
		return Vector2.Dot( delta.Normal, target.Rotation.Forward ) > 0.45f; // A little above 90 degrees
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceStab < 1f )
			return;

		if ( Input.Down( InputButton.PrimaryAttack ) )
		{
			using ( LagCompensation() )
			{
				TimeSinceStab = 0;
				MeleeAttack( 100f, 8f );
			}
		}
		else if ( Input.Released( InputButton.SecondaryAttack ) )
		{
			using ( LagCompensation() )
			{
				Throw();
			}
		}
	}

	public override bool CanCarry( Player carrier )
	{
		return !_isThrown && base.CanCarry( carrier );
	}

	private void MeleeAttack( float range, float radius )
	{
		Owner.SetAnimParameter( "b_attack", true );
		SwingEffects();
		PlaySound( SwingSound );

		var endPosition = Owner.EyePosition + Owner.EyeRotation.Forward * range;

		var trace = Trace.Ray( Owner.EyePosition, endPosition )
			.UseHitboxes( true )
			.Ignore( Owner )
			.Radius( radius )
			.Run();

		if ( !trace.Hit )
			return;

		trace.Surface.DoBulletImpact( trace );

		if ( !IsServer )
			return;

		var damage = NormalDamage;

		if ( trace.Entity is Player player )
		{
			if ( CanBackstab( Owner.Position, player ) )
				damage = BackstabDamage;

			player.DistanceToAttacker = 0;
			PlaySound( FleshHit );
			Owner.Inventory.DropActive();
			Delete();
		}

		var damageInfo = DamageInfo.Generic( damage )
			.WithPosition( trace.EndPosition )
			.UsingTraceResult( trace )
			.WithAttacker( Owner )
			.WithWeapon( this )
			.WithFlag( DamageFlags.Slash );

		trace.Entity.TakeDamage( damageInfo );
	}

	private void Throw()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition )
			.Ignore( Owner )
			.Run();

		_thrower = Owner;
		_isThrown = true;
		_thrownFrom = Owner.Position;
		_gravityModifier = 0;

		if ( !IsServer )
			return;

		if ( IsActiveCarriable )
			Owner.Inventory.DropActive();
		else
			Owner.Inventory.Drop( this );

		Position = trace.EndPosition;
		Rotation = PreviousOwner.EyeRotation * _throwRotation;
		Velocity = PreviousOwner.EyeRotation.Forward * (1250f + PreviousOwner.Velocity.Length);

		EnableTouch = false;
		PhysicsEnabled = false;
	}

	[ClientRpc]
	protected void SwingEffects()
	{
		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( !_isThrown )
			return;

		var oldPosition = Position;
		var newPosition = Position;
		newPosition += Velocity * Time.Delta;

		_gravityModifier += 8;
		newPosition -= new Vector3( 0f, 0f, _gravityModifier * Time.Delta );

		var trace = Trace.Ray( Position, newPosition )
			.Radius( 0f )
			.UseHitboxes()
			.WithAnyTags( "solid" )
			.Ignore( _thrower )
			.Ignore( this )
			.Run();

		Position = trace.EndPosition;
		Rotation = Rotation.From( trace.Direction.EulerAngles ) * _throwRotation;

		if ( !trace.Hit )
			return;

		switch ( trace.Entity )
		{
			case Player player:
			{
				trace.Surface.DoBulletImpact( trace );

				var damageInfo = DamageInfo.Generic(
					damage: CanBackstab( _thrownFrom, player ) ? BackstabDamage : NormalDamage
				).WithPosition( trace.EndPosition )
					.UsingTraceResult( trace )
					.WithFlag( DamageFlags.Slash )
					.WithAttacker( _thrower )
					.WithWeapon( this );

				player.DistanceToAttacker = _thrownFrom.Distance( player.Position ).SourceUnitsToMeters();
				player.TakeDamage( damageInfo );

				Delete();

				break;
			}
			case WorldEntity:
			{
				if ( Vector3.GetAngle( trace.Normal, trace.Direction ) < 120 )
					goto default;

				trace.Surface.DoBulletImpact( trace );

				Position -= trace.Direction * 4f; // Make the knife stuck in the terrain.			

				break;
			}
			default:
			{
				Position = oldPosition - trace.Direction * 5;
				Velocity = trace.Direction * 500f * PhysicsBody.Mass;
				PhysicsEnabled = true;
				break;
			}
		}

		EnableTouch = true;
		_isThrown = false;
	}
}
