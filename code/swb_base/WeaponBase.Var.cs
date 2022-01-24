﻿using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

/* 
 * Weapon base properties/fields
*/

namespace SWB_Base
{
    public enum HoldType
    {
        Pistol = 1,
        Rifle = 2,
        Shotgun = 3,
        Carry = 4,
        Fists = 5
    }

    public partial class WeaponBase
    {
        // Virtual
        public virtual int Bucket => 1; // Inventory slot position
        public virtual int BucketWeight => 100; // Inventory slot position weight ( higher = more important )
        public virtual bool CanDrop => true; // Can manually drop weapon
        public virtual bool DropWeaponOnDeath => true; // Drop the weapon on death
        public virtual bool BulletCocking => true; // Can bullets be cocked in the barrel? ( clip ammo + 1 )
        public virtual bool BarrelSmoking => true; // Should the barrel smoke after heavy weapon usage?
        public virtual string FreezeViewModelOnZoom => null; // Some weapons have looping idle animations -> force spam another animation to "freeze" it
        public virtual int FOV => 65; // Default FOV
        public virtual int ZoomFOV => 65; // FOV while zooming
        public virtual float TuckRange => 30; // Range that tucking should be enabled (set to -1 to disable tucking)
        public virtual HoldType HoldType => HoldType.Pistol; // Thirdperson holdtype
        public virtual string HandsModelPath => ""; // Path to the hands model (will be bonemerged with viewmodel, leave empty to disable)
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl"; // Path to the view model
        public virtual AngPos ViewModelOffset => new(); // Offset the viewmodel with an angle (not available in ModelDoc)
        public virtual string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl"; // Path to the world model
        public virtual string Icon => ""; // Path to an image that represent the weapon on the HUD
        public virtual float WalkAnimationSpeedMod => 1; // Procedural animation speed ( lower is slower )
        public virtual float AimSensitivity => 0.85f; // Aim sensitivity while zooming ( lower is slower )
        public virtual float PrimaryDelay => -1; // Delay before firing when the primary attack button is pressed
        public virtual float SecondaryDelay => -1; // Delay before firing when the secondary attack button is pressed

        // Properties
        public string PrintName { get { return ClassInfo.Title; } }

        public List<AnimatedAction> AnimatedActions { get; set; } // Extra actions that use certain key combinations to trigger animations

        public List<AttachmentCategory> AttachmentCategories { get; set; } // List of all weapon attachments

        public AngPos ZoomAnimData { get; set; } // Data used for setting the weapon to its zoom position

        public AngPos RunAnimData { get; set; } // Data used for setting the weapon to its run position

        public AngPos CustomizeAnimData { get; set; } // Data used for setting the weapon to its customization position

        public UISettings UISettings { get; set; } = new UISettings();

        [Net]
        public WeaponInfo General { get; set; } = new WeaponInfo(); // General data

        [Net]
        public ClipInfo Primary { get; set; } = new ClipInfo(); // Primary attack data

        [Net]
        public ClipInfo Secondary { get; set; } = null; // Secondary attack data ( setting this will disable weapon zooming )

        public TimeSince TimeSincePrimaryAttack { get; set; }

        public TimeSince TimeSinceSecondaryAttack { get; set; }

        public TimeSince TimeSinceReload { get; set; }

        public TimeSince TimeSinceDeployed { get; set; }

        public bool IsReloading { get; set; }

        public bool IsZooming { get; set; }

        public bool IsScoped { get; set; }

        public bool IsRunning { get; set; }

        public bool IsCustomizing { get; set; } // Client only

        public bool IsAnimating { get; set; }

        [Net]
        public bool InBoltBack { get; set; }

        public int InstanceID { get; protected set; }

        public PickupTrigger PickupTrigger { get; protected set; }

        public BaseViewModel HandsModel;

        public StatModifier InitialStats { get; protected set; }

        // Private
        private bool doRecoil = false;
        private int burstCount = 0;

        private int barrelHeat = 0;
        private TimeSince TimeSinceFired;
    }
}
