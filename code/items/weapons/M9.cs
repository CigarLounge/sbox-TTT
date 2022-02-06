using System;
using System.Collections.Generic;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_m9", Title = "M9" )]
	[Shop( SlotType.Secondary, 100 )]
	[Spawnable]
	[Precached( "models/weapons/v_vertec.vmdl", "models/weapons/w_vertec.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_vertec.vmdl" )]
	public class M9 : WeaponBase, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( M9 ) );
		public Type DroppedType => typeof( SMGAmmo );

		public override int Bucket => 1;
		public override HoldType HoldType => HoldType.Pistol;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_vertec.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_vertec.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;

		public M9()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.3f,
				ReloadTime = 1.6f,
				ReloadEmptyTime = 2.9f
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
				ShootSound = "vertec_fire-1",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = 0
			};

			ZoomAnimData = new AngPos
			{
				Angle = new Angles( -0.62f, 5.1f, -0.4f ),
				Pos = new Vector3( -4.155f, -2f, 0.916f )
			};

			RunAnimData = new AngPos
			{
				Angle = new Angles( -30, 0, 0 ),
				Pos = new Vector3( 0, -3, -8 )
			};

			CustomizeAnimData = new AngPos
			{
				Angle = new Angles( -19.2f, 69.6f, 0f ),
				Pos = new Vector3( 10.4f, -16.2f, 2.6f )
			};
		}

		public override void Simulate( Client client )
		{
			WeaponGenerics.Simulate( client, Primary, DroppedType );
			base.Simulate( client );
		}

		public string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}


