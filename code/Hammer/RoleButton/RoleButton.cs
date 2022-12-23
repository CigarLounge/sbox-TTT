using Editor;
using Sandbox;

namespace TTT;

[ClassName( "ttt_role_button" )]
[Description( "On-screen button that can be pressed by users with the specified role." )]
[HammerEntity]
[Sphere( "radius" )]
[Title( "Role Button" )]
public partial class RoleButton : Entity
{
	[Title( "Role" )]
	[Description( "The name of the `Role` to check for. Ex. Innocent, Detective, Traitor" )]
	[Net, Property]
	public string RoleName { get; private set; } = "Traitor";

	[Description( "On screen tooltip shown on button." )]
	[Net, Property]
	public string Description { get; private set; }

	[Description( "Maximum radius a player can see and activate a button. Buttons are fully opaque within 512 units." )]
	[Net, Property]
	public int Radius { get; private set; } = 100;

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
	public TimeUntil NextUse { get; private set; }

	[Net]
	public bool IsRemoved { get; private set; }

	protected Output OnPressed { get; set; }
	public bool IsDisabled => !NextUse || Locked || IsRemoved;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	public bool CanUse( Player player )
	{
		if ( IsDisabled )
			return false;

		return RoleName == "All" || player.Role == RoleName;
	}

	[Input]
	public void Press( Entity activator )
	{
		if ( activator is not Player player )
			return;

		if ( CanUse( player ) )
		{
			_ = OnPressed.Fire( player );

			if ( RemoveOnPress )
			{
				IsRemoved = true;
				return;
			}

			NextUse = Delay;
		}
	}

	[Input]
	public void Lock()
	{
		Locked = true;
	}

	[Input]
	public void Unlock()
	{
		Locked = false;
	}

	[Input]
	public void Toggle()
	{
		Locked = !Locked;
	}
}
