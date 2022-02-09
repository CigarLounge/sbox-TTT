using System;
using System.Collections.Generic;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_weapon_silencedpistol", Title = "Silenced Pistol" )]
	[Shop( SlotType.Secondary, 100, new Type[] { typeof( TraitorRole ) } )]
	[Precached( "models/weapons/v_vertec.vmdl", "models/weapons/w_vertec.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_vertec.vmdl" )]
	public class SilencedPistol : M9
	{
		public new ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( SilencedPistol ) );

		public SilencedPistol()
		{
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
	}
}
