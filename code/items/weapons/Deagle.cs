using System;
using System.Collections.Generic;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_deagle", Title = "Deagle" )]
	[Shop( SlotType.Secondary, 100 )]
	[Precached( "weapons/swb/pistols/deagle/v_deagle.vmdl", "weapons/swb/pistols/deagle/w_deagle.vmdl" )]
	[Hammer.EditorModel( "weapons/swb/pistols/deagle/w_deagle.vmdl" )]
	public class Deagle : WeaponBase, ICarriableItem, IEntityHint
	{
		public virtual ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( Deagle ) );
		public Type DroppedType => typeof( MagnumAmmo );

		public override int Bucket => 1;
		public override HoldType HoldType => HoldType.Pistol;
		public override string HandsModelPath => "weapons/swb/hands/rebel/v_hands_rebel.vmdl";
		public override string ViewModelPath => "weapons/swb/pistols/deagle/v_deagle.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "weapons/swb/pistols/deagle/w_deagle.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;

		public Deagle()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.3f,
				ReloadTime = 1.6f
			};

			Primary = new ClipInfo
			{
				Ammo = 7,
				AmmoType = AmmoType.SMG,
				ClipSize = 7,

				BulletSize = 6f,
				Damage = 30f,
				Force = 2f,
				Spread = 0.06f,
				Recoil = 4f,
				RPM = 300,
				FiringType = FiringType.semi,
				ScreenShake = new ScreenShake
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 1.0f,
					Rotation = 0.5f
				},

				DryFireSound = "dryfire_pistol-1",
				ShootSound = "deagle.fire",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = 0
			};
		}

		public override void Simulate( Client client )
		{
			WeaponGenerics.Simulate( client, Primary, DroppedType );
			base.Simulate( client );
		}

		public float HintDistance => TTTPlayer.INTERACT_DISTANCE;
		public virtual string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}
