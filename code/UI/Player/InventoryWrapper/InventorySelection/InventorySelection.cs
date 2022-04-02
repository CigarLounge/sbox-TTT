using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public class InventorySelection : Panel
{
	private readonly Dictionary<Carriable, InventorySlot> _entries = new();

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

	public override void Tick()
	{
		base.Tick();

		var player = (Local.Pawn as Player).CurrentPlayer;

		foreach ( var carriable in player.Inventory )
		{
			if ( !_entries.ContainsKey( carriable ) && carriable.Owner is not null )
				_entries[carriable] = AddInventorySlot( carriable );
		}

		var activeItem = player.ActiveChild as Carriable;
		var activeItemTitle = activeItem is not null ? Asset.GetInfo<CarriableInfo>( activeItem ).Title : string.Empty;
		foreach ( var slot in _entries.Values )
		{
			if ( !player.Inventory.Contains( slot.Carriable ) )
			{
				_entries.Remove( slot.Carriable );
				slot?.Delete();
			}

			slot.SetClass( "rounded-top", slot == Children.First() as InventorySlot );
			slot.SetClass( "rounded-bottom", slot == Children.Last() as InventorySlot );

			slot.SlotLabel.SetClass( "rounded-top-left", slot == Children.First() as InventorySlot );
			slot.SlotLabel.SetClass( "rounded-bottom-left", slot == Children.Last() as InventorySlot );

			slot.SetClass( "active", slot.Carriable.IsActiveChild );
			slot.SetClass( "opacity-heavy", slot.Carriable.IsActiveChild );

			slot.UpdateSlotText( slot.Carriable.SlotText );
		}

		SortChildren( ( p1, p2 ) =>
		{
			var s1 = p1 as InventorySlot;
			var s2 = p2 as InventorySlot;

			int result = s1.Carriable.Info.Slot.CompareTo( s2.Carriable.Info.Slot );
			return result != 0
				? result
				: string.Compare( s1.Carriable.Info.Title, s2.Carriable.Info.Title, StringComparison.Ordinal );
		} );

		this.Enabled( Children.Any() );
	}

	private InventorySlot AddInventorySlot( Carriable carriable )
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
		var player = Local.Pawn as Player;
		if ( !player.IsAlive() )
			return;

		if ( Children is null || !Children.Any() )
			return;

		var childrenList = Children.ToList();

		var activeCarriable = player.ActiveChild as Carriable;
		int keyboardIndexPressed = GetKeyboardNumberPressed( input );
		if ( keyboardIndexPressed != -1 )
		{
			List<Carriable> weaponsOfSlotTypeSelected = new();
			int activeCarriableOfSlotTypeIndex = -1;

			for ( int i = 0; i < childrenList.Count; ++i )
			{
				if ( childrenList[i] is InventorySlot slot )
				{
					if ( (int)slot.Carriable.Info.Slot == keyboardIndexPressed - 1 )
					{
						// Using the keyboard index the user pressed, find all carriables that
						// have the same slot type as the index.
						// Ex. "3" pressed, find all carriables with slot type "3".
						weaponsOfSlotTypeSelected.Add( slot.Carriable );

						if ( slot.Carriable == activeCarriable )
						{
							// If the current active carriable has the same slot type as
							// the keyboard index the user pressed
							activeCarriableOfSlotTypeIndex = weaponsOfSlotTypeSelected.Count - 1;
						}
					}
				}
			}

			if ( activeCarriable is null || activeCarriableOfSlotTypeIndex == -1 )
			{
				// The user isn't holding an active carriable, or is holding a weapon that has a different
				// hold type than the one selected using the keyboard. We can just select the first weapon.
				input.ActiveChild = weaponsOfSlotTypeSelected.FirstOrDefault();
			}
			else
			{
				// The user is holding a weapon that has the same hold type as the keyboard index the user pressed.
				// Find the next possible weapon within the hold types.

				activeCarriableOfSlotTypeIndex = GetNextWeaponIndex( activeCarriableOfSlotTypeIndex, weaponsOfSlotTypeSelected.Count );
				input.ActiveChild = weaponsOfSlotTypeSelected[activeCarriableOfSlotTypeIndex];
			}
		}

		int mouseWheelIndex = input.MouseWheel;
		if ( mouseWheelIndex != 0 )
		{
			int activeCarriableIndex = childrenList.FindIndex( ( p ) =>
				 p is InventorySlot slot && slot.Carriable == activeCarriable );

			int newSelectedIndex = NormalizeSlotIndex( -mouseWheelIndex + activeCarriableIndex, childrenList.Count - 1 );
			input.ActiveChild = (childrenList[newSelectedIndex] as InventorySlot)?.Carriable;
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
				return i;
		}

		return -1;
	}
}
