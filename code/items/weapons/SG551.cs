using System;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_sg551", Title = "SG-551" )]
	[Shop( SlotType.Primary, 100 )]
	[Spawnable]
	[Precached( "models/weapons/v_sg551.vmdl", "models/weapons/w_sg551.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_sg551.vmdl" )]
	public class SG551 : WeaponBase, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( SG551 ) );
		public Type DroppedType => typeof( RifleAmmo );

		public override float TuckRange => -1f;
		public override int Bucket => 1;
		public override HoldType HoldType => HoldType.Rifle;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_sg551.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_sg551.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;

		public SG551()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.5f,
				ReloadTime = 3.0f
			};

			Primary = new ClipInfo
			{
				Ammo = 10,
				AmmoType = AmmoType.Rifle,
				ClipSize = 10,

				BulletSize = 4f,
				Damage = 37f,
				Force = 1f,
				Spread = 0.0001f,
				Recoil = 5f,
				RPM = 141,
				FiringType = FiringType.auto,
				ScreenShake = new ScreenShake
				{
					Length = 0.4f,
					Speed = 3.0f,
					Size = 0.35f,
					Rotation = 0.35f
				},

				DryFireSound = "dryfire_rifle-1",
				ShootSound = "sg551_fire-1",

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
		public string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}
