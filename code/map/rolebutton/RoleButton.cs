using Sandbox;

namespace TTT;

[Hammer.EntityTool( "Role Button", "TTT", "Used to provide an on-screen button for a role to activate." )]
[Library( "ttt_role_button" )]
public partial class RoleButton : Entity
{
	[Net, Property( "Check Value", "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	public string Role { get; set; } = "Traitor";

	[Net, Property( "Description", "On screen tooltip shown on button." )]
	public string Description { get; private set; }

	[Net, Property( "Range", "Maximum range a player can see and activate a button. Buttons are fully opaque within 512 units." )]
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

	private TimeUntil NextUse { get; set; }

	protected Output OnPressed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always; // Make sure our clients receive the button entity.
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
			IsDelayed = false;
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
}
