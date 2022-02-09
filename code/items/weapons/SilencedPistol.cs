using System;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_weapon_silencedpistol", Title = "Silenced Pistol" )]
	[Shop( SlotType.Secondary, 100, new Type[] { typeof( TraitorRole ) } )]
	[Precached( "models/weapons/v_vertec-silenced.vmdl", "models/weapons/w_vertec-silenced.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_vertec-silenced.vmdl" )]
	public class SilencedPistol : M9
	{
		public override string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		public override ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( SilencedPistol ) );

		public override string ViewModelPath => "models/weapons/v_vertec-silenced.vmdl";
		public override string WorldModelPath => "models/weapons/w_vertec-silenced.vmdl";

		public SilencedPistol()
		{
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
				ShootSound = "vertec_fire_silenced-1",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = 0
			};
		}
	}
}
