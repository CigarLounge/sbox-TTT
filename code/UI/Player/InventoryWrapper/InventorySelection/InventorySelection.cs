using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

public partial class InventorySelection : Panel
{
	private static readonly InputButton[] _slotInputButtons = new[]
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

	public static int GetKeyboardNumberPressed()
	{
		for ( var i = 0; i < _slotInputButtons.Length; i++ )
			if ( Input.Pressed( _slotInputButtons[i] ) )
				return i;

		return -1;
	}

	[Event.Client.BuildInput]
	private void BuildInput()
	{
		if ( Game.LocalPawn is not Player player || !player.IsAlive() )
			return;

		if ( !Children.Any() )
			return;

		if ( QuickChat.IsShowing )
			return;

		var childrenList = Children.ToList();

		var activeCarriable = player.ActiveCarriable;

		var keyboardIndexPressed = GetKeyboardNumberPressed();

		if ( keyboardIndexPressed != -1 )
		{
			List<Carriable> weaponsOfSlotTypeSelected = new();
			var activeCarriableOfSlotTypeIndex = -1;

			for ( var i = 0; i < childrenList.Count; ++i )
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
				player.ActiveChildInput = weaponsOfSlotTypeSelected.FirstOrDefault();
			}
			else
			{
				// The user is holding a weapon that has the same hold type as the keyboard index the user pressed.
				// Find the next possible weapon within the hold types.
				activeCarriableOfSlotTypeIndex = GetNextWeaponIndex( activeCarriableOfSlotTypeIndex, weaponsOfSlotTypeSelected.Count );
				player.ActiveChildInput = weaponsOfSlotTypeSelected[activeCarriableOfSlotTypeIndex];
			}
		}

		var mouseWheelIndex = Input.MouseWheel;
		if ( mouseWheelIndex != 0 )
		{
			var activeCarriableIndex = childrenList.FindIndex( ( p ) =>
				 p is InventorySlot slot && slot.Carriable == activeCarriable );

			var newSelectedIndex = ClampSlotIndex( -mouseWheelIndex + activeCarriableIndex, childrenList.Count - 1 );
			player.ActiveChildInput = (childrenList[newSelectedIndex] as InventorySlot)?.Carriable;
		}
	}

	// Keyboard selection can only increment the index by 1.
	private int GetNextWeaponIndex( int index, int count )
	{
		return ClampSlotIndex( index + 1, count - 1 );
	}

	private int ClampSlotIndex( int index, int maxIndex )
	{
		return index > maxIndex ? 0 : index < 0 ? maxIndex : index;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CameraMode.Target.Inventory.HashCombine( carriable => carriable.NetworkIdent ) );
	}
}
