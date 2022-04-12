using Sandbox;

namespace TTT;

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

		Transmit = TransmitType.Always;
	}

	public void OnSecond()
	{
		Host.AssertServer();

		if ( HasDelay && IsDelayed && !IsRemoved && NextUse <= 0 )
			IsDelayed = false;
	}

	[Input]
	public void Press( Player activator )
	{
		Host.AssertServer();

		if ( !IsDisabled )
		{
			OnPressed.Fire( activator );

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
