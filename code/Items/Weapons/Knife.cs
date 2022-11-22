using Sandbox;
using SandboxEditor;

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

	public override void Simulate( Client client )
	{
		if ( TimeSinceStab < 1f )
			return;

		if ( Input.Down( InputButton.PrimaryAttack ) )
		{
			using ( LagCompensation() )
			{
				TimeSinceStab = 0;
				StabAttack( 100f, 8f );
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
			.UseHitboxes( true )
			.Ignore( Owner )
			.Radius( radius )
			.Run();

		if ( !trace.Hit )
			return;

		trace.Surface.DoBulletImpact( trace );

		if ( !IsServer )
			return;

		var stabDamage = Game.EnableBackstabs ? 50 : 100;
		var damageInfo = DamageInfo.Generic( stabDamage )
			.WithPosition( trace.EndPosition )
			.UsingTraceResult( trace )
			.WithAttacker( Owner )
			.WithWeapon( this )
			.WithFlag( DamageFlags.Slash );

		if ( trace.Entity is Player otherPlayer )
		{
			otherPlayer.DistanceToAttacker = 0;
			PlaySound( FleshHit );

			// Discard all z values to simplify to 2D.
			var toTarget = (otherPlayer.Position - Owner.Position).WithZ( 0 ).Normal;
			var ownerForward = Owner.EyeRotation.Forward.WithZ( 0 ).Normal;
			var targetForward = otherPlayer.EyeRotation.Forward.WithZ( 0 ).Normal;

			var behindDot = Vector3.Dot( toTarget, targetForward );
			var facingDot = Vector3.Dot( toTarget, ownerForward );
			var viewDot = Vector3.Dot( targetForward, ownerForward );

			var isBackstab = (behindDot > 0.0f && facingDot > 0.5f && viewDot > -0.3f);

			if ( isBackstab && Game.EnableBackstabs )
				damageInfo.Damage *= 2;

			if ( otherPlayer.Health - stabDamage <= 0 )
			{
				Owner.Inventory.DropActive();
				Delete();
			}
		}

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

				var damageInfo = DamageInfo.Generic( 100f )
					.WithPosition( trace.EndPosition )
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
