using System;

using Sandbox;
using SWB_Base;
using TTT.Player;

namespace TTT.Items
{
	public static partial class WeaponGenerics
	{
		private const int AMMO_DROP_POSITION_OFFSET = 50;
		private const int AMMO_DROP_VELOCITY = 500;

		public static string PickupText( string LibraryTitle )
		{
			return $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to pickup {LibraryTitle}";
		}

		public static void Simulate( Client client, ClipInfo clip, Type ammoType )
		{
			if ( Host.IsClient )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Drop ) && Input.Down( InputButton.Run ) && clip.Ammo > 0 && clip.InfiniteAmmo == 0 )
				{
					if ( ammoType != null )
					{
						TTTAmmo ammoBox = Utils.GetObjectByType<TTTAmmo>( ammoType );

						ammoBox.Position = client.Pawn.EyePosition + client.Pawn.EyeRotation.Forward * AMMO_DROP_POSITION_OFFSET;
						ammoBox.Rotation = client.Pawn.EyeRotation;
						ammoBox.Velocity = client.Pawn.EyeRotation.Forward * AMMO_DROP_VELOCITY;
						ammoBox.SetCurrentAmmo( clip.Ammo );
					}

					clip.Ammo -= clip.Ammo;
				}
			}
		}

		public static void Tick( TTTPlayer player, ICarriableItem item )
		{
			if ( Host.IsClient )
			{
				return;
			}

			if ( player.LifeState != LifeState.Alive )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Use ) )
				{
					if ( player.Inventory.Active is IItem activeItem && activeItem.GetItemData().SlotType == item.GetItemData().SlotType )
					{
						player.Inventory.DropActive();
					}

					player.AddItem( item, true, false );
				}
			}
		}
	}
}
