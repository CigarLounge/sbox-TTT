using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_role_button" )]
[Description( "On-screen button that can be pressed by users with the specified role." )]
[HammerEntity]
[Sphere( "radius" )]
[Title( "Role Button" )]
public partial class RoleButton : Entity
{
	[Description( "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	[Title( "Role" )]
	[Net, Property]
	public string RoleName { get; private set; } = "Traitor";

	[Description( "On screen tooltip shown on button." )]
	[Net, Property]
	public string Description { get; private set; }

	[Description( "Maximum radius a player can see and activate a button. Buttons are fully opaque within 512 units." )]
	[Net, Property]
	public int Radius { get; private set; } = 1024;

	[Description( "Delay in seconds until button will reactive once triggered. Hammer doesn't like using decimal values, so this only takes integers." )]
	[Property]
	public int Delay { get; private set; } = 1;

	[Description( "Only allows button to be pressed once per round." )]
	[Property]
	public bool RemoveOnPress { get; private set; } = false;

	[Description( "Is the button locked? If enabled, button needs to be unlocked with the `Unlock` or `Toggle` input." )]
	[Net, Property]
	public bool Locked { get; private set; } = false;

	[Net]
	public bool IsDelayed { get; set; }

	[Net]
	public bool IsRemoved { get; set; }

	private TimeUntil NextUse { get; set; }
	protected Output OnPressed { get; set; }
	public bool IsDisabled => Locked || IsDelayed || IsRemoved;
	public bool HasDelay => Delay > 0.0f;

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
	public void Press( Entity activator )
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

		Locked = true;
	}

	[Input]
	public void Unlock()
	{
		Host.AssertServer();

		Locked = false;
	}

	[Input]
	public void Toggle()
	{
		Host.AssertServer();

		Locked = !Locked;
	}
}
