using System;

using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	public static partial class WeaponGenerics
	{
		private const int AMMO_DROP_POSITION_OFFSET = 50;
		private const int AMMO_DROP_VELOCITY = 500;

		public static void Simulate( Client owner, Type AmmoType, SWB_Base.ClipInfo clip )
		{
			if ( Input.Pressed( InputButton.Drop ) && Input.Down( InputButton.Run ) && clip.Ammo > 0 && clip.InfiniteAmmo == 0 )
			{
				if ( Host.IsServer && AmmoType != null )
				{
					TTTAmmo ammoBox = Utils.GetObjectByType<TTTAmmo>( AmmoType );

					ammoBox.Position = owner.Pawn.EyePos + owner.Pawn.EyeRot.Forward * AMMO_DROP_POSITION_OFFSET;
					ammoBox.Rotation = owner.Pawn.EyeRot;
					ammoBox.Velocity = owner.Pawn.EyeRot.Forward * AMMO_DROP_VELOCITY;
					ammoBox.SetCurrentAmmo( clip.Ammo );
				}

				clip.Ammo -= clip.Ammo;
			}
		}

		public static string PickupText( string LibraryTitle )
		{
			return $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to pickup {LibraryTitle}";
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

					player.Inventory.TryAdd( item, deleteIfFails: false, makeActive: true );
				}
			}
		}
	}
}
