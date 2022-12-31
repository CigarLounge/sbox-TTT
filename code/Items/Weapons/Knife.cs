using Editor;
using Sandbox;

namespace TTT;

[Category( "Weapons" )]
[ClassName( "ttt_weapon_knife" )]
[HammerEntity]
[Title( "Knife" )]
public partial class Knife : Carriable
{
	[Net, Local, Predicted]
	public TimeSince TimeSinceStab { get; private set; }

	private const string SwingSound = "knife_swing-1";
	private const string FleshHit = "knife_flesh_hit-1";

	private bool _isThrown = false;
	private Player _thrower;
	private Vector3 _thrownFrom;
	private Rotation _throwRotation = Rotation.From( new Angles( 90, 0, 0 ) );
	private float _gravityModifier;

	public override void Simulate( IClient client )
	{
		if ( TimeSinceStab < 1f )
			return;

		if ( Input.Down( InputButton.PrimaryAttack ) )
		{
			using ( LagCompensation() )
			{
				TimeSinceStab = 0;
				StabAttack( 35f, 8f );
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

	private void StabAttack( float range, float radius )
	{
		Owner.SetAnimParameter( "b_attack", true );
		SwingEffects();
		PlaySound( SwingSound );

		var endPosition = Owner.EyePosition + Owner.EyeRotation.Forward * range;

		var trace = Trace.Ray( Owner.EyePosition, endPosition )
			.Ignore( Owner )
			.Radius( radius )
			.Run();

		if ( !trace.Hit )
			return;

		trace.Surface.DoBulletImpact( trace );

		if ( !Game.IsServer )
			return;

		var damageInfo = DamageInfo.Generic( GameManager.KnifeBackstabs ? 50 : 100 )
			.UsingTraceResult( trace )
			.WithAttacker( Owner )
			.WithTags( Strings.Tags.Slash, "silent" )
			.WithWeapon( this );

		if ( trace.Entity is Player player )
		{
			player.DistanceToAttacker = 0;

			PlaySound( FleshHit );

			if ( GameManager.KnifeBackstabs && IsBehindAndFacingTarget( player ) )
				damageInfo.Damage *= 2;
		}

		trace.Entity.TakeDamage( damageInfo );

		if ( trace.Entity is Player && !trace.Entity.IsAlive() )
		{
			Owner.Inventory.DropActive();
			Delete();
		}
	}

	private bool IsBehindAndFacingTarget( Player target )
	{
		var toOwner = new Vector2( Owner.WorldSpaceBounds.Center - target.WorldSpaceBounds.Center ).Normal;
		var ownerForward = new Vector2( Owner.EyeRotation.Forward ).Normal;
		var targetForward = new Vector2( target.EyeRotation.Forward ).Normal;

		var behindDot = Vector2.Dot( toOwner, targetForward );
		var facingDot = Vector2.Dot( toOwner, ownerForward );
		var viewDot = Vector2.Dot( targetForward, ownerForward );

		return behindDot < 0.0f && facingDot < -0.5f && viewDot > -0.3f;
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

		if ( !Game.IsServer )
			return;

		if ( IsActive )
			Owner.Inventory.DropActive();
		else
			Owner.Inventory.Drop( this );

		EnableTouch = false;
		PhysicsEnabled = false;

		Position = trace.EndPosition;
		Rotation = PreviousOwner.EyeRotation * _throwRotation;
		Velocity = PreviousOwner.EyeRotation.Forward * (600f + PreviousOwner.Velocity.Length);
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

				var damageInfo = DamageInfo.Generic( 100f )
					.UsingTraceResult( trace )
					.WithAttacker( _thrower )
					.WithTags( Strings.Tags.Slash, "silent" )
					.WithWeapon( this );

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
