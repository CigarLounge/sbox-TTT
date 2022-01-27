﻿using Sandbox;

namespace SWB_Base
{
	internal class Commands
	{
		[ServerCmd( "swb_editor_model", Help = "Opens the model editor" )]
		public static void OpenModelEditor()
		{
			Client client = ConsoleSystem.Caller;

			if ( client != null )
			{
				var player = client.Pawn as PlayerBase;
				player.ToggleModelEditor();
			}
		}

		[ServerCmd( "swb_editor_attachment", Help = "Opens the attachment editor" )]
		public static void OpenAttachmentEditor()
		{
			Client client = ConsoleSystem.Caller;

			if ( client != null )
			{
				var player = client.Pawn as PlayerBase;
				player.ToggleAttachmentEditor();
			}
		}

		[ServerCmd( "swb_attachment_equip", Help = "Equips an attachment by name" )]
		public static void EquipAttachmentCMD( string name, bool enabled )
		{
			Client client = ConsoleSystem.Caller;

			if ( client != null )
			{
				var player = client.Pawn as PlayerBase;
				var activeWeapon = player.ActiveChild as WeaponBase;
				if ( activeWeapon == null ) return;

				var activeAttachment = activeWeapon.GetActiveAttachment( name );

				if ( enabled && activeAttachment == null && activeWeapon.GetAttachment( name ) != null )
				{
					// Attach
					activeWeapon.EquipAttachmentSV( name );
				}
				else if ( !enabled && activeAttachment != null )
				{
					// Detach
					activeWeapon.UnequipAttachmentSV( name );
				}
			}
		}
	}
}
