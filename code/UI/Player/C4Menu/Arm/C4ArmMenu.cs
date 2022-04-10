using System;
using Sandbox.UI;
using TTT.Entities;

namespace TTT.UI;

[UseTemplate]
public class C4ArmMenu : EntityHintPanel
{
	public int Timer { get; set; } = 45;

	private Label TimerDisplay { get; set; }
	private Label Wires { get; set; }

	private readonly C4 _c4;

	public C4ArmMenu( C4 c4 )
	{
		_c4 = c4;
	}

	public void Arm()
	{
		C4.ArmC4( _c4.NetworkIdent, Timer );
	}

	public void Pickup()
	{
		C4.Pickup( _c4.NetworkIdent );
	}

	public void Destroy()
	{
		C4.DeleteC4( _c4.NetworkIdent );
	}

	public override void Tick()
	{
		TimerDisplay.Text = TimeSpan.FromSeconds( Timer ).ToString( "mm':'ss" );
		Wires.Text = $"{C4.GetBadWireCount( Timer )} of the 6 wires will cause instant detonation during defusal";
	}
}
