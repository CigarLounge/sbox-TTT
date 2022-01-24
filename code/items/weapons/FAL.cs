using System;
using System.Collections.Generic;

using Sandbox;

using SWB_Base;

namespace TTT.Items
{
    [Library("weapon_fal")]
    [Weapon(SlotType = SlotType.Primary)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/swb/hands/rebel/v_hands_rebel.vmdl", "weapons/swb/rifles/fal/v_fal.vmdl", "weapons/swb/rifles/fal/w_fal.vmdl")]
    [Hammer.EditorModel("weapons/swb/rifles/fal/w_fal.vmdl")]
    public class FAL : TTTWeaponBase
    {
        public override int Bucket => 3;
        public override HoldType HoldType => HoldType.Rifle;
        public override string HandsModelPath => "weapons/swb/hands/rebel/v_hands_rebel.vmdl";
        public override string ViewModelPath => "weapons/swb/rifles/fal/v_fal.vmdl";
        public override AngPos ViewModelOffset => new()
        {
            Angle = new Angles(0, -5, 0),
            Pos = new Vector3(-5, 0, 0)
        };
        public override string WorldModelPath => "weapons/swb/rifles/fal/w_fal.vmdl";
        public override string Icon => "/swb_weapons/textures/fal.png";
        public override int FOV => 75;
        public override int ZoomFOV => 75;
        public override float WalkAnimationSpeedMod => 0.85f;

        public FAL()
        {
            DroppedType = typeof(RifleAmmo);

            General = new WeaponInfo
            {
                DrawTime = 1f,
                ReloadTime = 2.03f,
                ReloadEmptyTime = 2.67f
            };

            Primary = new ClipInfo
            {
                Ammo = 20,
                AmmoType = AmmoType.Rifle,
                ClipSize = 20,

                BulletSize = 4f,
                Damage = 15f,
                Force = 3f,
                Spread = 0.1f,
                Recoil = 0.5f,
                RPM = 600,
                FiringType = FiringType.semi,
                ScreenShake = new ScreenShake
                {
                    Length = 0.5f,
                    Speed = 4.0f,
                    Size = 0.5f,
                    Rotation = 0.5f
                },

                DryFireSound = "swb_rifle.empty",
                ShootSound = "fal.fire",

                BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
                MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

                InfiniteAmmo = InfiniteAmmoType.normal
            };

            ZoomAnimData = new AngPos
            {
                Angle = new Angles(-0.1f, 4.95f, -1f),
                Pos = new Vector3(-5f, -4.211f, 0.75f)
            };

            RunAnimData = new AngPos
            {
                Angle = new Angles(10, 40, 0),
                Pos = new Vector3(5, 0, 0)
            };

            CustomizeAnimData = new AngPos
            {
                Angle = new Angles(-2.25f, 51.84f, 0f),
                Pos = new Vector3(11.22f, -4.96f, 1.078f)
            };
        }
    }
}
