using System;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_p90", Title = "P90" )]
	[Shop( SlotType.Primary, 100 )]
	[Precached( "models/weapons/v_p90.vmdl", "models/weapons/w_p90.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_p90.vmdl" )]
	public class P90 : WeaponBase, ICarriableItem, IEntityHint
	{
		public ItemData Data { get; set; }
		public Type DroppedType => typeof( SMGAmmo );

		public override float TuckRange => -1f;
		public override int Bucket => 1;
		public override HoldType HoldType => HoldType.Rifle;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_p90.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_p90.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;

		public P90()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.0f,
				ReloadTime = 2.8f
			};

			Primary = new ClipInfo
			{
				Ammo = 50,
				AmmoType = AmmoType.SMG,
				ClipSize = 50,

				BulletSize = 4f,
				Damage = 6f,
				Force = 1f,
				Spread = 0.03f,
				Recoil = 1f,
				RPM = 700,
				FiringType = FiringType.auto,
				ScreenShake = new ScreenShake
				{
					Length = 0.4f,
					Speed = 3.0f,
					Size = 0.35f,
					Rotation = 0.35f
				},

				DryFireSound = "dryfire_pistol-1",
				ShootSound = "p90_fire-1",

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
		public virtual string TextOnTick => WeaponGenerics.PickupText( Data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}


