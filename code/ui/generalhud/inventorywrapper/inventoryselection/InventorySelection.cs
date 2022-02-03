using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Events;
using TTT.Items;
using TTT.Player;

namespace TTT.UI
{
	public class InventorySelection : Panel
	{
		private readonly Dictionary<ICarriableItem, InventorySlot> _entries = new();

		private readonly InputButton[] _slotInputButtons = new[]
		{
			InputButton.Slot0,
			InputButton.Slot1,
			InputButton.Slot2,
			InputButton.Slot3,
			InputButton.Slot4,
			InputButton.Slot5,
			InputButton.Slot6,
			InputButton.Slot7,
			InputButton.Slot8,
			InputButton.Slot9
		};

		public InventorySelection() : base()
		{
			StyleSheet.Load( "/ui/generalhud/inventorywrapper/inventoryselection/InventorySelection.scss" );

			AddClass( "opacity-heavy" );
			AddClass( "text-shadow" );

			if ( Local.Pawn is not TTTPlayer player )
			{
				return;
			}
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not TTTPlayer player )
			{
				return;
			}

			// This code sucks. I'm forced to due this because of...
			// https://github.com/Facepunch/sbox-issues/issues/1324
			foreach ( var item in player.CurrentPlayer.Inventory.List )
			{
				if ( item is ICarriableItem carriable )
				{
					if ( !_entries.ContainsKey( carriable ) && item.Owner != null )
					{
						_entries[carriable] = CarriableItemPickup( carriable );
					}
				}
			}

			ICarriableItem activeItem = player.CurrentPlayer.ActiveChild as ICarriableItem;
			foreach ( var slot in _entries.Values )
			{
				if ( !player.CurrentPlayer.Inventory.Contains( slot.Carriable as Entity ) )
				{
					_entries.Remove( slot.Carriable );
					slot?.Delete();
				}

				slot.SetClass( "rounded-top", slot == Children.First() as InventorySlot );
				slot.SetClass( "rounded-bottom", slot == Children.Last() as InventorySlot );

				slot.SlotLabel.SetClass( "rounded-top-left", slot == Children.First() as InventorySlot );
				slot.SlotLabel.SetClass( "rounded-bottom-left", slot == Children.Last() as InventorySlot );

				slot.SetClass( "active", slot.Carriable.LibraryTitle == activeItem?.LibraryTitle );
				slot.SetClass( "opacity-heavy", slot.Carriable.LibraryTitle == activeItem?.LibraryTitle );

				if ( slot.Carriable.SlotType != SlotType.Melee && slot.Carriable is SWB_Base.WeaponBase weapon )
				{
					slot.UpdateAmmo( FormatAmmo( weapon, player.CurrentPlayer.AmmoCount( weapon.Primary.AmmoType ) ) );
				}
			}

			SortChildren( ( p1, p2 ) =>
			{
				InventorySlot s1 = p1 as InventorySlot;
				InventorySlot s2 = p2 as InventorySlot;

				int result = s1.Carriable.SlotType.CompareTo( s2.Carriable.SlotType );
				return result != 0
					? result
					: string.Compare( s1.Carriable.LibraryTitle, s2.Carriable.LibraryTitle, StringComparison.Ordinal );
			} );

			this.Enabled( Children.Any() );
		}

		private InventorySlot CarriableItemPickup( ICarriableItem carriable )
		{
			var inventorySlot = new InventorySlot( this, carriable );
			AddChild( inventorySlot );
			return inventorySlot;
		}

		/// <summary>
		/// IClientInput implementation, calls during the client input build.
		/// You can both read and write to input, to affect what happens down the line.
		/// </summary>
		[Event.BuildInput]
		private void ProcessClientInventorySelectionInput( InputBuilder input )
		{
			if ( Local.Pawn is not TTTPlayer player || player.IsSpectatingPlayer )
			{
				return;
			}

			if ( Children == null || !Children.Any() )
			{
				return;
			}

			List<Panel> childrenList = Children.ToList();

			ICarriableItem activeCarriable = Local.Pawn.ActiveChild as ICarriableItem;

			int keyboardIndexPressed = GetKeyboardNumberPressed( input );
			if ( keyboardIndexPressed != -1 )
			{
				List<ICarriableItem> weaponsOfSlotTypeSelected = new();
				int activeCarriableOfSlotTypeIndex = -1;

				for ( int i = 0; i < childrenList.Count; ++i )
				{
					if ( childrenList[i] is InventorySlot slot )
					{
						if ( (int)slot.Carriable.SlotType == keyboardIndexPressed )
						{
							// Using the keyboard index the user pressed, find all carriables that
							// have the same slot type as the index.
							// Ex. "3" pressed, find all carriables with slot type "3".
							weaponsOfSlotTypeSelected.Add( slot.Carriable );

							if ( slot.Carriable.LibraryTitle == activeCarriable?.LibraryTitle )
							{
								// If the current active carriable has the same slot type as
								// the keyboard index the user pressed
								activeCarriableOfSlotTypeIndex = weaponsOfSlotTypeSelected.Count - 1;
							}
						}
					}
				}

				if ( activeCarriable == null || activeCarriableOfSlotTypeIndex == -1 )
				{
					// The user isn't holding an active carriable, or is holding a weapon that has a different
					// hold type than the one selected using the keyboard. We can just select the first weapon.
					input.ActiveChild = weaponsOfSlotTypeSelected.FirstOrDefault() as Entity;
				}
				else
				{
					// The user is holding a weapon that has the same hold type as the keyboard index the user pressed.
					// Find the next possible weapon within the hold types.
					input.ActiveChild = weaponsOfSlotTypeSelected[GetNextWeaponIndex( activeCarriableOfSlotTypeIndex, weaponsOfSlotTypeSelected.Count )] as Entity;
				}
			}

			int mouseWheelIndex = input.MouseWheel;
			if ( mouseWheelIndex != 0 )
			{
				int activeCarriableIndex = childrenList.FindIndex( ( p ) =>
					 p is InventorySlot slot && slot.Carriable.LibraryTitle == activeCarriable?.LibraryTitle );

				int newSelectedIndex = NormalizeSlotIndex( -mouseWheelIndex + activeCarriableIndex, childrenList.Count - 1 );
				input.ActiveChild = (childrenList[newSelectedIndex] as InventorySlot)?.Carriable as Entity;
			}
		}

		// Keyboard selection can only increment the index by 1.
		private int GetNextWeaponIndex( int index, int count )
		{
			return NormalizeSlotIndex( index + 1, count - 1 );
		}

		private int NormalizeSlotIndex( int index, int maxIndex )
		{
			return index > maxIndex ? 0 : index < 0 ? maxIndex : index;
		}

		private int GetKeyboardNumberPressed( InputBuilder input )
		{
			for ( int i = 0; i < _slotInputButtons.Length; i++ )
			{
				if ( input.Pressed( _slotInputButtons[i] ) )
				{
					return i;
				}
			}

			return -1;
		}

		private static string FormatAmmo( SWB_Base.WeaponBase weapon, int ammoCount )
		{
			if ( weapon.Primary.InfiniteAmmo != 0 )
			{
				return $"{weapon.Primary.AmmoType} + ∞";
			}

			return $"{weapon.Primary.Ammo} + {ammoCount}";
		}

		private class InventorySlot : Panel
		{
			public ICarriableItem Carriable { get; init; }
			public Label SlotLabel;
			private readonly Label _ammoLabel;

			public InventorySlot( Panel parent, ICarriableItem carriable ) : base( parent )
			{
				Parent = parent;
				Carriable = carriable;

				AddClass( "background-color-primary" );

				SlotLabel = Add.Label( ((int)carriable.SlotType).ToString() );
				SlotLabel.AddClass( "slot-label" );

				Add.Label( carriable.LibraryTitle );

				_ammoLabel = Add.Label();

				if ( Local.Pawn is TTTPlayer player )
				{
					if ( carriable.SlotType != SlotType.Melee && carriable is SWB_Base.WeaponBase weapon )
					{
						_ammoLabel.Text = FormatAmmo( weapon, player.AmmoCount( weapon.Primary.AmmoType ) );
						_ammoLabel.AddClass( "ammo-label" );
					}
				}
			}

			public override void Tick()
			{
				base.Tick();

				if ( Local.Pawn is TTTPlayer player )
				{
					SlotLabel.Style.BackgroundColor = player.Team.Color;
				}
			}

			public void UpdateAmmo( string ammoText )
			{
				_ammoLabel.Text = ammoText;
			}
		}
	}
}
