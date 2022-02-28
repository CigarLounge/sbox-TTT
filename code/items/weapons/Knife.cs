﻿using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_knife.vmdl" )]
[Library( "ttt_weapon_knife", Title = "Knife" )]
public partial class Knife : Carriable
{
	[Net, Predicted] public TimeSince TimeSinceStab { get; set; }
	private Vector3 _thrownFrom;
	private bool _hasLanded = true;
	private Player _thrower;
	private bool _isThrown = false;
	private float _gravityModifier;

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

		if ( trace.Entity is Player player )
		{
			player.LastDistanceToAttacker = Utils.SourceUnitsToMeters( Owner.Position.Distance( player.Position ) );
			PlaySound( RawStrings.FleshHit );
			Owner.Inventory.DropActive();
			Delete();
		}

		trace.Entity.TakeDamage( info );
	}

	public void Throw()
	{
		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition ).Ignore( Owner ).Run();
		_thrower = Owner;
		_isThrown = true;
		_hasLanded = false;
		_thrownFrom = Owner.Position;
		_gravityModifier = 0;

		if ( !IsServer )
		{
			Owner.Inventory.Active = null;
			return;
		}

		var owner = Owner;
		Owner.Inventory.DropActive();
		PhysicsEnabled = false;
		MoveType = MoveType.None;
		Rotation = owner.EyeRotation;
		Position = trace.EndPosition;
		Velocity = owner.EyeRotation.Forward * 1000f;
		EnableTouch = false;
	}

	[ClientRpc]
	protected void SwingEffects()
	{
		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	[Event.Tick.Server]
	public void ServerTick()
	{
		if ( !_isThrown || _hasLanded )
			return;

		var oldPosition = Position;
		var newPosition = Position;
		newPosition += Velocity * Time.Delta;

		_gravityModifier += 8;
		newPosition -= new Vector3( 0f, 0f, _gravityModifier * Time.Delta );

		var trace = Trace.Ray( Position, newPosition )
			.Ignore( _thrower )
			.Radius( 0 )
			.WorldAndEntities()
			.Run();

		Position = trace.EndPosition;
		Rotation = Rotation.From( trace.Direction.EulerAngles );

		if ( !trace.Hit )
			return;

		if ( trace.Entity is Player player && player != _thrower )
		{
			var damageInfo = new DamageInfo()
					.WithFlag( DamageFlags.Slash )
					.WithAttacker( _thrower )
					.WithWeapon( this );

			trace.Surface.DoBulletImpact( trace );
			Velocity = Vector3.Zero;
			damageInfo.Damage = 100f;
			player.LastDistanceToAttacker = Utils.SourceUnitsToMeters( _thrownFrom.Distance( player.Position ) );
			player.TakeDamage( damageInfo );

			Delete();
		}
		else if ( trace.Entity.IsWorld && Vector3.GetAngle( trace.Normal, trace.Direction ) > 115 )
		{
			trace.Surface.DoBulletImpact( trace );
			Position -= trace.Direction * 6f;
			MoveType = MoveType.None;
		}
		else
		{
			Position = oldPosition;
			MoveType = MoveType.Physics;
			PhysicsEnabled = true;
			ApplyLocalImpulse( (trace.Normal - trace.Direction) * 50f );
		}

		_isThrown = false;
		_hasLanded = true;
		EnableTouch = true;
	}
}
