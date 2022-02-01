using System;
using System.Collections.Generic;

using Sandbox;

using SWB_Base;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_weapon_silenceddeagle", Title = "Silenced Deagle" )]
	[Weapon( SlotType = SlotType.Secondary )]
	[Buyable( Price = 100 )]
	[Shops( new Type[] { typeof( TraitorRole ) } )]
	[Precached( "weapons/swb/hands/rebel/v_hands_rebel.vmdl", "weapons/swb/pistols/deagle/v_deagle.vmdl", "weapons/swb/pistols/deagle/w_deagle.vmdl" )]
	[Hammer.EditorModel( "weapons/swb/pistols/deagle/w_deagle.vmdl" )]
	public class SilencedDeagle : Deagle
	{
		public SilencedDeagle()
		{
			DroppedType = typeof( RevolverAmmo );

			General = new WeaponInfo
			{
				DrawTime = 1f,
				ReloadTime = 1.8f,
				ReloadEmptyTime = 2.9f
			};

			Primary = new ClipInfo
			{
				Ammo = 7,
				AmmoType = AmmoType.Revolver,
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

				DryFireSound = "swb_pistol.empty",
				ShootSound = "deagle.fire",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_medium.vpcf",

				InfiniteAmmo = 0
			};

			ZoomAnimData = new AngPos
			{
				Angle = new Angles( 0.25f, 4.95f, -0.4f ),
				Pos = new Vector3( -5f, -2f, 2.45f )
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

			// Attachments //
			AttachmentCategories = new List<AttachmentCategory>()
			{
				new AttachmentCategory
				{
					Name = AttachmentCategoryName.Muzzle,
					BoneOrAttachment = "muzzle",
					Attachments = new List<AttachmentBase>()
					{
						new SWB_Base.Attachments.PistolSilencer
						{
							Enabled = true,
							MuzzleFlashParticle = "particles/swb/muzzle/flash_medium_silenced.vpcf",
							ShootSound = "swb_heavy.silenced.fire",
							ViewParentBone = "talon",
							ViewTransform = new Transform
							{
								Position = new Vector3(0f, 2.6f, 14.8f),
								Rotation = Rotation.From(new Angles(-90f, 0f, 90f)),
								Scale = 9f
							},
							WorldParentBone = "talon",
							WorldTransform = new Transform {
								Position = new Vector3(1f, 4.4f, 16.5f),
								Rotation = Rotation.From(new Angles(-90f, 0f, 0f)),
								Scale = 9f
							},
						}
					}
				},
			};
		}
	}
}
