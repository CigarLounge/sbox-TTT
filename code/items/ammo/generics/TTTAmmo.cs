using System;

using Sandbox;

using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Hammer.Skip]
	public abstract partial class TTTAmmo : Prop, IEntityHint
	{
		/// <summary>
		/// The library name of the ammo.
		/// </summary>
		public string LibraryName { get; set; }

		/// <summary>
		/// The ammo type to use.
		/// </summary>
		public virtual SWB_Base.AmmoType AmmoType { get; set; }

		/// <summary>
		/// Amount of Ammo within Entity.
		/// </summary>
		public virtual int Amount { get; set; }

		/// <summary>
		/// Maximum amount of ammo player can carry in their inventory.
		/// </summary>
		public virtual int Max { get; set; }

		[Net]
		private int CurrentAmmo { get; set; }
		private int AmmoEntMax { get; set; }

		/// <summary>
		/// Fired when a player picks up any amount of ammo from the entity.
		/// </summary>
		protected Output OnPickup { get; set; }

		public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";

		public TTTAmmo() : base()
		{
			LibraryName = Utils.GetLibraryName( GetType() );
		}

		public override void Spawn()
		{
			base.Spawn();

			SetModel( ModelPath );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			CollisionGroup = CollisionGroup.Weapon;
			SetInteractsAs( CollisionLayer.Debris );

			AmmoEntMax = Amount;
			CurrentAmmo = Amount;

			Tags.Add( IItem.ITEM_TAG );
		}

		public void SetCurrentAmmo( int ammo )
		{
			CurrentAmmo = ammo;
		}

		public override void TakeDamage( DamageInfo info )
		{
			PhysicsBody body = info.Body;

			if ( !body.IsValid() )
			{
				body = PhysicsBody;
			}

			if ( body.IsValid() && !info.Flags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				body.ApplyImpulseAt( info.Position, info.Force * 100 );
			}
		}

		public float HintDistance => 80f;


		public string TextOnTick => $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to pickup {LibraryName}";

		public bool CanHint( TTTPlayer client )
		{
			return true;
		}

		public EntityHintPanel DisplayHint( TTTPlayer client )
		{
			return new Hint( TextOnTick );
		}

		public void Tick( TTTPlayer player )
		{
			if ( IsClient )
			{
				return;
			}

			if ( player.LifeState != LifeState.Alive )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.Use ) )
				{
					return;
				}

				bool hasWeaponOfAmmoType = false;
				for ( int i = 0; i < player.Inventory.Count(); ++i )
				{
					if ( player.Inventory.GetSlot( i ) is SWB_Base.WeaponBase weapon )
					{
						if ( weapon.Primary.AmmoType == AmmoType )
						{
							hasWeaponOfAmmoType = true;
							break;
						}
					}
				}

				if ( !hasWeaponOfAmmoType )
				{
					return;
				}

				int playerAmount = player.AmmoCount( AmmoType );

				if ( Max < playerAmount + Math.Ceiling( CurrentAmmo * 0.25 ) )
				{
					return;
				}

				int amountGiven = Math.Min( CurrentAmmo, Max - playerAmount );
				player.GiveAmmo( AmmoType, amountGiven );
				CurrentAmmo -= amountGiven;
				OnPickup.Fire( player );

				if ( CurrentAmmo <= 0 || Math.Ceiling( AmmoEntMax * 0.25 ) > CurrentAmmo )
				{
					Delete();
				}
			}
		}
	}
}
