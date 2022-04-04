using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_newtonlauncher", Title = "Newton Launcher" )]
public partial class NewtonLauncher : Weapon
{
	[Net, Local, Predicted]
	private int Charge { get; set; }

	public override string SlotText => $"{Charge}%";

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		// While we have no viewmodel let's just show the world model.
		EnableHideInFirstPerson = false;
	}
}
