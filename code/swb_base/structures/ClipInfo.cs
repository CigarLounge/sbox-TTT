﻿using Sandbox;

namespace SWB_Base
{
    public enum FiringType
    {
        semi,
        auto,
        burst
    }

    public partial class ClipInfo : BaseNetworkable
    {
        [Net, Predicted]
        public int Ammo { get; set; } = 10; // Amount of ammo in the clip
        [Net]
        public AmmoType AmmoType { get; set; } = AmmoType.Pistol; // Type of ammo
        [Net]
        public int ClipSize { get; set; } = 10; // Size of the clip

        // Shooting
        [Net]
        public int Bullets { get; set; } = 1; // Amount of bullets per shot
        [Net]
        public float BulletSize { get; set; } = 0.1f; // Bullet size
        [Net]
        public float Damage { get; set; } = 5; // Bullet damage
        [Net]
        public float Force { get; set; } = 0.1f; // Bullet force
        [Net]
        public float Spread { get; set; } = 0.1f; // Weapon spread
        [Net]
        public float Recoil { get; set; } = 0.1f; // Weapon recoil
        [Net]
        public int RPM { get; set; } = 200; // Firing speed ( higher is faster )
        [Net]
        public FiringType FiringType { get; set; } = FiringType.semi; // Firing type
        [Net]
        public ScreenShake ScreenShake { get; set; } // Screenshake per shot

        // Strings
        [Net]
        public string ShootAnim { get; set; } = "fire"; // Shooting animation
        [Net]
        public string DryFireSound { get; set; } // Firing sound when clip is empty
        [Net]
        public string ShootSound { get; set; } // Firing sound
        [Net]
        public string BulletEjectParticle { get; set; } // Particle that should be used for bullet ejection
        [Net]
        public string MuzzleFlashParticle { get; set; } // Particle that should be used for the muzzle flash
        [Net]
        public string BarrelSmokeParticle { get; set; } = "particles/swb/muzzle/barrel_smoke.vpcf"; // Particle that should be used for the barrel smoke
        [Net]
        public string BulletTracerParticle { get; set; } = "particles/swb/tracer/tracer_medium.vpcf"; // Particle that should be used for the barrel smoke

        // Extra
        [Net]
        public InfiniteAmmoType InfiniteAmmo { get; set; } = InfiniteAmmoType.normal; // If the weapon should have infinite ammo
    }
}
