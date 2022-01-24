using System;
using System.Collections.Generic;

using Sandbox;

using SWB_Base;

namespace TTT.Items
{
    [Library("weapon_l96a1")]
    [Weapon(SlotType = SlotType.Primary)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/swb/hands/rebel/v_hands_rebel.vmdl", "weapons/swb/snipers/l96a1/v_l96a1.vmdl", "weapons/swb/snipers/l96a1/w_l96a1.vmdl",
    "particles/swb/muzzle/flash_large.vpcf", "particles/swb/tracer/tracer_large.vpcf")]
    [Hammer.EditorModel("weapons/swb/snipers/l96a1/w_l96a1.vmdl")]
    public class L96A1 : TTTWeaponBaseSniper
    {
        public override int Bucket => 5;
        public override HoldType HoldType => HoldType.Rifle;
        public override string HandsModelPath => "weapons/swb/hands/rebel/v_hands_rebel.vmdl";
        public override string ViewModelPath => "weapons/swb/snipers/l96a1/v_l96a1.vmdl";
        public override AngPos ViewModelOffset => new()
        {
            Angle = new Angles(0, -5, 0),
            Pos = new Vector3(-5, 0, 0)
        };
        public override string WorldModelPath => "weapons/swb/snipers/l96a1/w_l96a1.vmdl";
        public override string Icon => "/swb_weapons/textures/l96a1.png";
        public override int FOV => 75;
        public override int ZoomFOV => 75;
        public override float WalkAnimationSpeedMod => 0.8f;
        public override float AimSensitivity => 0.25f;

        public override string LensTexture => "/materials/swb/scopes/swb_lens_hunter.png";
        public override string ScopeTexture => "/materials/swb/scopes/swb_scope_hunter.png";
        public override string ZoomInSound => "swb_sniper.zoom_in";
        public override float ZoomAmount => 15f;
        public override bool UseRenderTarget => false;

        public L96A1()
        {
            DroppedType = typeof(SniperAmmo);

            UISettings = new UISettings
            {
                ShowCrosshair = false
            };

            General = new WeaponInfo
            {
                DrawTime = 0.5f,
                ReloadTime = 1.8f,

                BoltBackTime = 1.6f,
                BoltBackEjectDelay = 0.5f
            };

            Primary = new ClipInfo
            {
                Ammo = 5,
                AmmoType = AmmoType.Sniper,
                ClipSize = 5,

                BulletSize = 5f,
                Damage = 100f,
                Force = 7f,
                Spread = 0.75f,
                Recoil = 2f,
                RPM = 125,
                FiringType = FiringType.semi,
                ScreenShake = new ScreenShake
                {
                    Length = 0.5f,
                    Speed = 4.0f,
                    Size = 1f,
                    Rotation = 0.5f
                },

                DryFireSound = "swb_sniper.empty",
                ShootSound = "l96a1.fire",

                BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
                MuzzleFlashParticle = "particles/swb/muzzle/flash_large.vpcf",
                BulletTracerParticle = "particles/swb/tracer/tracer_large.vpcf",

                InfiniteAmmo = InfiniteAmmoType.normal
            };

            ZoomAnimData = new AngPos
            {
                Angle = new Angles(0f, 2.5f, -2f),
                Pos = new Vector3(-6f, 4f, -2f)
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
