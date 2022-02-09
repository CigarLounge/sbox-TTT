using System;
using System.Collections.Generic;
using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.Roles;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_silentfox", Title = "Silent Fox" )]
	[Shop( SlotType.Primary, 100, new Type[] { typeof( TraitorRole ) } )]
	[Precached( "models/weapons/v_mp7.vmdl", "models/weapons/w_mp7.vmdl" )]
	[Hammer.EditorModel( "models/weapons/w_mp7.vmdl" )]
	public class SilentFox : MP7
	{
		public new ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( SilentFox ) );

		public SilentFox()
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
							ViewParentBone = "mp7_rootbone",
							ViewTransform = new Transform { Position = new Vector3(-2.25f, -0.07f, 12.5f), Rotation = Rotation.From(new Angles(-90f, 0f, 0f)), Scale = 7.5f },
							WorldParentBone = "hold_R",
							WorldTransform = new Transform { Position = new Vector3(14.5f, 0f, 2.5f), Rotation = Rotation.From(new Angles(0f, 0f, 0f)), Scale = 7.5f },
						}
					}
				},
			};
		}
	}
}


