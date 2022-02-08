using System;
using System.Collections.Generic;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_silencedpistol", Title = "Silenced Pistol" )]
	[Shop( SlotType.Secondary, 100 )]
	[Precached( "models/weapons/v_vertec.vmdl", "models/weapons/w_vertec.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_vertec.vmdl" )]
	public class SilencedPistol : WeaponBase, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( SilencedPistol ) );
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

		public SilencedPistol()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.3f,
				ReloadTime = 1.6f
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

			ZoomAnimData = new AngPos { Angle = new Angles( -1.52f, 5.04f, 0f ), Pos = new Vector3( -2.669f, 0f, 0.599f ) };

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
							ShootSound = "vertec_fire_silenced-1",
							ViewParentBone = "barrel",
							ViewTransform = new Transform { Position = new Vector3(-0.3f, 0f, 8.5f), Rotation = Rotation.From(new Angles(-90f, 0f, 0f)), Scale = 7f },
							WorldParentBone = "ak47_bolt",
							WorldTransform = new Transform { Position = new Vector3(8.65f, -0.5f, 0f), Rotation = Rotation.From(new Angles(0f, 0f, 0f)), Scale = 7f },
						}
					}
				},
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
