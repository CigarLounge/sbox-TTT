using System;
using Sandbox;

using SWB_Base;

using TTT.Player;
using TTT.Roles;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_knife", Title = "Knife" )]
	[Shop( SlotType.Melee, 100, new Type[] { typeof( TraitorRole ) } )]
	[Precached( "models/weapons/v_knife.vmdl", "models/weapons/w_knife.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_knife.vmdl" )]
	public class Knife : WeaponBaseMelee, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( Knife ) );

		public override int Bucket => 0;
		public override HoldType HoldType => HoldType.Fists; // just use fists for now
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_knife.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -15, 0 ),
			Pos = new Vector3( -4, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_knife.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override float WalkAnimationSpeedMod => 1.25f;

		public override string SwingAnimationHit => "fire";
		public override string SwingAnimationMiss => "fire";
		public override string StabAnimationHit => "stab";
		public override string StabAnimationMiss => "stab_miss";
		public override string SwingSound => "knife_flesh_hit-1";
		public override string StabSound => "bayonet.stab";
		public override string MissSound => "knife_swing-1";
		public override string HitWorldSound => "bayonet.hitwall";
		public override float SwingSpeed => 1f;
		public override float StabSpeed => 1f;
		public override float SwingDamage => 200f;
		public override float StabDamage => 50f;
		public override float SwingForce => 25f;
		public override float StabForce => 50f;
		public override float DamageDistance => 35f;
		public override float ImpactSize => 10f;

		private readonly float _primaryEntitySpeed = 200f;
		private Vector3 _entityVelocity = new( 0, 0, 10 );
		private Vector3 _entitySpawnOffset = new( 0, 10, 50 );
		private Angles _entityAngles = new( 90, -60, 10 );

		public Knife()
		{

		}

		public override void MeleeAttack( float damage, float force, string hitAnimation, string missAnimation, string sound )
		{
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			var hitEntity = true;
			var pos = Owner.EyePosition;
			var forward = Owner.EyeRotation.Forward;
			var trace = Trace.Ray( pos, pos + forward * DamageDistance )
				.Ignore( this )
				.Ignore( Owner )
				.Size( ImpactSize )
				.Run();

			if ( !trace.Entity.IsValid() || trace.Entity.IsWorld )
			{
				hitAnimation = missAnimation;
				sound = !trace.Entity.IsValid() ? MissSound : HitWorldSound;
				hitEntity = false;
			}

			DoMeleeEffects( hitAnimation, sound );
			(Owner as AnimEntity).SetAnimBool( "b_attack", true );

			if ( !hitEntity || !IsServer ) return;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( trace.EndPos, forward * force, damage )
					.UsingTraceResult( trace )
					.WithAttacker( Owner )
					.WithFlag( DamageFlags.Slash )
					.WithWeapon( this );

				trace.Entity.TakeDamage( damageInfo );

				if ( trace.Entity is TTTPlayer )
				{
					Owner.Inventory.Drop( this );
					Delete();
				}
			}
		}

		public override void AttackSecondary()
		{
			if ( Host.IsServer )
			{
				using ( Prediction.Off() )
				{
					ThrowKnife( new ThrownKnife() );
					Owner.Inventory.Drop( this );
					Delete();
				}
			}
		}

		public void ThrowKnife( ThrownKnife knife )
		{
			if ( !string.IsNullOrEmpty( WorldModelPath ) )
				knife.SetModel( WorldModelPath );

			knife.Owner = Owner;
			knife.Position = MathUtil.RelativeAdd( Position, _entitySpawnOffset, Owner.EyeRotation );
			knife.Rotation = Owner.EyeRotation * Rotation.From( _entityAngles );
			knife.RemoveDelay = -1;
			knife.UseGravity = true;
			knife.Speed = _primaryEntitySpeed;
			knife.IsSticky = true;
			knife.Damage = SwingDamage;
			knife.Force = Primary.Force;
			knife.StartVelocity = MathUtil.RelativeAdd( Vector3.Zero, _entityVelocity, Owner.EyeRotation );
			knife.Start();
		}

		public override void Simulate( Client client )
		{
			WeaponGenerics.Simulate( client, Primary, null );
			base.Simulate( client );
		}

		public float HintDistance => TTTPlayer.INTERACT_DISTANCE;
		public string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}

	public class ThrownKnife : FiredEntity, IEntityHint
	{
		public float HintDistance => TTTPlayer.INTERACT_DISTANCE;

		public string TextOnTick => WeaponGenerics.PickupText( "Knife" );
		public bool CanHint( TTTPlayer client ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer client ) { return new Hint( TextOnTick ); }

		private bool _hasLanded = false;

		public void Tick( TTTPlayer player )
		{
			if ( Host.IsClient )
			{
				return;
			}

			if ( player.LifeState != LifeState.Alive )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Use ) )
				{
					player.Inventory.Add( new Knife(), false, false );
					Delete();
				}
			}
		}

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			if ( !_hasLanded && eventData.Entity is TTTPlayer playerHit )
			{
				DamageInfo info = new()
				{
					Damage = Damage,
					Force = 50f,
					Attacker = Owner,
					Weapon = new Knife(),
					Flags = DamageFlags.Slash
				};

				Velocity = Vector3.Zero;
				Parent = playerHit;
				EnableAllCollisions = false;
				playerHit.TakeDamage( info );
			}

			_hasLanded = true;
		}
	}
}
