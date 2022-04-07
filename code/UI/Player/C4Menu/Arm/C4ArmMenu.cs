using System;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4ArmMenu : EntityHintPanel
{
	public int Timer { get; set; } = 45;

	private Label TimerDisplay { get; set; }

	private readonly C4Entity _c4;

	public C4ArmMenu( C4Entity c4 )
	{
		_c4 = c4;
	}

	public void Arm()
	{
		C4Entity.ArmC4( _c4.NetworkIdent, Timer );
	}

	public void Pickup()
	{
		C4Entity.Pickup( _c4.NetworkIdent );
	}

	public void Destroy()
	{
		C4Entity.DeleteC4( _c4.NetworkIdent );
	}

	public override void Tick()
	{
		TimerDisplay.Text = TimeSpan.FromSeconds( Timer ).ToString( "mm':'ss" );
	}
}
