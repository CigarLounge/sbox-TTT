using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_knife.vmdl" )]
[Library( "ttt_weapon_knife", Title = "Knife" )]
public partial class Knife : Carriable
{
	[Net, Predicted]
	public TimeSince TimeSinceStab { get; set; }

	private const string SwingSound = "knife_swing-1";
	private const string FleshHit = "knife_flesh_hit-1";

	private bool _isThrown = false;
	private Player _thrower;
	private Vector3 _thrownFrom;
	private Rotation _throwRotation = Rotation.From( new Angles( 90, 0, 0 ) );
	private float _gravityModifier;

	public override void Simulate( Client client )
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
		return !_isThrown && base.CanCarry( carrier );
	}

	private void MeleeAttack( float damage, float range, float radius )
	{
		TimeSinceStab = 0;

		Owner.SetAnimParameter( "b_attack", true );
		SwingEffects();
		PlaySound( SwingSound );

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

		var damageInfo = new DamageInfo()
			.WithPosition( trace.EndPosition )
			.UsingTraceResult( trace )
			.WithAttacker( Owner )
			.WithFlag( DamageFlags.Slash )
			.WithWeapon( this );

		damageInfo.Damage = damage;

		if ( trace.Entity is Player player )
		{
			player.LastDistanceToAttacker = 0;
			PlaySound( FleshHit );
			Owner.Inventory.DropActive();
			Delete();
		}

		trace.Entity.TakeDamage( damageInfo );
	}

	private void Throw()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition ).Ignore( Owner ).Run();
		_thrower = Owner;
		_isThrown = true;
		_thrownFrom = Owner.Position;
		_gravityModifier = 0;
		Owner.Inventory.SetActive( Owner.LastActiveChild );

		if ( !IsServer )
			return;

		if ( IsActiveChild )
			Owner.Inventory.DropActive();
		else
			Owner.Inventory.Drop( this );

		Rotation = PreviousOwner.EyeRotation * _throwRotation;
		Position = trace.EndPosition;
		MoveType = MoveType.None;
		PhysicsEnabled = false;
		Velocity = PreviousOwner.EyeRotation.Forward * (1250f + PreviousOwner.Velocity.Length);
		EnableTouch = false;
	}

	[ClientRpc]
	protected void SwingEffects()
	{
		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
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
			.HitLayer( CollisionLayer.Debris )
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

				var damageInfo = new DamageInfo()
					.WithPosition( trace.EndPosition )
					.UsingTraceResult( trace )
					.WithFlag( DamageFlags.Slash )
					.WithAttacker( _thrower )
					.WithWeapon( this );

				damageInfo.Damage = 100f;

				player.LastDistanceToAttacker = _thrownFrom.Distance( player.Position ).SourceUnitsToMeters();
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
				MoveType = MoveType.None;

				break;
			}
			default:
			{
				Position = oldPosition - trace.Direction * 5;
				MoveType = MoveType.Physics;
				PhysicsEnabled = true;
				Velocity = trace.Direction * 500f * PhysicsBody.Mass;

				break;
			}
		}

		_isThrown = false;
		EnableTouch = true;
	}
}
