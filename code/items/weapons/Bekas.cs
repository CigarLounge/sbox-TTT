using System;
using Sandbox;
using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_bekas", Title = "Bekas-M" )]
	[Shop( SlotType.Primary, 100 )]
	[Spawnable]
	[Precached( "models/weapons/v_bekas.vmdl", "models/weapons/w_bekas.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_bekas.vmdl" )]
	public class Bekas : WeaponBaseShotty, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( Bekas ) );
		public Type DroppedType => typeof( Bekas );

		public override int Bucket => 2;
		public override HoldType HoldType => HoldType.Shotgun;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_bekas.vmdl";
		public override string WorldModelPath => "models/weapons/w_bekas.vmdl";
		public override int FOV => 75;
		public override int ZoomFOV => 75;
		public override float WalkAnimationSpeedMod => 0.9f;

		public override float ShellReloadTimeStart => 0.33f;
		public override float ShellReloadTimeInsert => 0.54f;
		public override float ShellEjectDelay => 0.5f;

		public Bekas()
		{
			Primary = new ClipInfo
			{
				Ammo = 6,
				AmmoType = AmmoType.Shotgun,
				ClipSize = 6,

				Bullets = 8,
				BulletSize = 2f,
				Damage = 7f,
				Force = 3f,
				Spread = 0.3f,
				Recoil = 2f,
				RPM = 70,
				FiringType = FiringType.semi,
				ScreenShake = new ScreenShake
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 1.0f,
					Rotation = 0.5f
				},

				DryFireSound = "dryfire_rifle-1",
				ShootSound = "bekas_fire-1",

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
