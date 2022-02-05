using System;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_mp5", Title = "MP5" )]
	[Shop( SlotType.Primary, 100 )]
	[Spawnable]
	[Precached( "models/weapons/v_arms_devgru.vmdl", "models/weapons/v_mp5.vmdl", "models/weapons/w_mp5.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_mp5.vmdl" )]
	public class MP5 : WeaponBase, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( MP5 ) );
		public Type DroppedType => typeof( SMGAmmo );

		public override int Bucket => 1;
		public override HoldType HoldType => HoldType.Rifle;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_mp5.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_mp5.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;

		public MP5()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.2f,
				ReloadTime = 1.6f,
				ReloadEmptyTime = 2.47f
			};

			Primary = new ClipInfo
			{
				Ammo = 25,
				AmmoType = AmmoType.SMG,
				ClipSize = 25,

				BulletSize = 4f,
				Damage = 20f,
				Force = 3f,
				Spread = 0.08f,
				Recoil = 0.35f,
				RPM = 600,
				FiringType = FiringType.auto,
				ScreenShake = new ScreenShake
				{
					Length = 0.4f,
					Speed = 3.0f,
					Size = 0.35f,
					Rotation = 0.35f
				},

				DryFireSound = "swb_smg.empty",
				ShootSound = "ump45.fire",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = 0
			};

			ZoomAnimData = new AngPos
			{
				Angle = new Angles( -2.664f, 5.02f, 0f ),
				Pos = new Vector3( -2.545f, -0.4f, 0.715f )
			};

			RunAnimData = new AngPos
			{
				Angle = new Angles( 8.41f, 36.54f, 0f ),
				Pos = new Vector3( 9.937f, 0f, 1.137f )
			};

			CustomizeAnimData = new AngPos
			{
				Angle = new Angles( -3.71f, 48.72f, 0f ),
				Pos = new Vector3( 27.694f, -4.96f, 2.24f )
			};
		}

		public string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}


