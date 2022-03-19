using Sandbox;

namespace TTT;

[Library( "ttt_logic_button", Description = "Used to provide an onscreen button for a role to activate." )]
public partial class LogicButton : Entity
{
	[Property( "Check Value", "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	public string Role { get; set; }

	[Net, Property( "Description", "On screen tooltip shown on button." )]
	public string Description { get; private set; }

	[Property( "Range", "Maximum range a player can see and activate a button. Buttons are fully opaque within 512 units." )]
	public int Range { get; private set; } = 1024;

	[Property( "Delay", "Delay in seconds until button will reactive once triggered. Hammer doesn't like using decimal values, so this only takes integers." )]
	public int Delay { get; private set; } = 1;

	[Property( "Remove On Press", "Only allows button to be pressed once per round." )]
	public bool RemoveOnPress { get; private set; } = false;

	[Property( "Locked", "Is the button locked? If enabled, button needs to be unlocked with the `Unlock` or `Toggle` input." )]
	public bool Locked { get; private set; } = false;

	// Tracks button state.
	[Net]
	public bool IsLocked { get; set; }

	[Net]
	public bool IsDelayed { get; set; }

	[Net]
	public bool IsRemoved { get; set; }

	public bool IsDisabled => IsLocked || IsDelayed || IsRemoved;

	public bool HasDelay => Delay > 0.0f;

	private RealTimeUntil NextUse { get; set; }

	protected Output OnPressed { get; set; }

	public LogicButton()
	{
		Transmit = TransmitType.Always; // Make sure our clients receive the button entity.

		if ( IsServer )
			Cleanup();
	}

	// (Re)initialize our variables to default. Runs at preround as well as during construction
	public void Cleanup()
	{
		Host.AssertServer();

		IsLocked = Locked;
		IsDelayed = false;
		IsRemoved = false;
		NextUse = 0;
	}

	public void OnSecond() // Hijack the round timer to tick on every second. No reason to tick any faster.
	{
		Host.AssertServer();

		if ( HasDelay && IsDelayed && !IsRemoved && NextUse <= 0 ) // Check timer if button has delayed, no reason to check if button is removed.
		{
			IsDelayed = false;
		}
	}

	[Input]
	public void Press( Player activator )
	{
		Host.AssertServer();

		if ( !IsDisabled ) // Make sure button is not delayed, locked or removed.
		{
			OnPressed.Fire( activator ); // Fire Hammer IO

			if ( RemoveOnPress )
			{
				IsRemoved = true;
				return;
			}

			if ( Delay > 0.0f )
			{
				IsDelayed = true;
				NextUse = Delay;
				return;
			}
		}
	}

	// Hammer IO
	[Input]
	public void Lock()
	{
		Host.AssertServer();
		IsLocked = true;
	}

	[Input]
	public void Unlock()
	{
		Host.AssertServer();
		IsLocked = false;
	}

	[Input]
	public void Toggle()
	{
		Host.AssertServer();
		IsLocked = !IsLocked;
	}

	public bool CanUse() => !IsDisabled;

	// Convert starter data to struct to network to clients for UI display.
	public LogicButtonData PackageData()
	{
		return new LogicButtonData()
		{
			NetworkIdent = NetworkIdent,
			Range = Range,
			Position = Position,
			IsDisabled = IsDisabled,
		};
	}
}

// Package up our data nice and neat for transmission to the client.
public struct LogicButtonData
{
	public int NetworkIdent;
	public int Range;
	public Vector3 Position;
	public bool IsDisabled;
}
