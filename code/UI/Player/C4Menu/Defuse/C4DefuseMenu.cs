using System;
using System.Linq;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4DefuseMenu : EntityHintPanel
{
	private readonly C4Entity _c4;

	private Label TimerDisplay { get; set; }
	private Panel Wires { get; set; }
	private Button PickupBtn { get; set; }
	private Button DestroyBtn { get; set; }

	public C4DefuseMenu( C4Entity c4 )
	{
		_c4 = c4;

		var wires = Wires.Children.ToList();
		for ( int i = 0; i < wires.Count; ++i )
		{
			var wire = wires[i] as Wire;
			wire.Number.Text = $"{i + 1}";
			wire.AddEventListener( "onclick", () =>
			{
				wire.Style.Opacity = 0;

				PickupBtn.RemoveClass( "inactive" );
				DestroyBtn.RemoveClass( "inactive" );
			} );
		}
	}

	public override void Tick()
	{
		if ( _c4 != null )
			TimerDisplay.Text = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
	}

	public void Defuse( int wire )
	{
		C4Entity.Defuse( wire, _c4.NetworkIdent );
	}

	public void Pickup()
	{
		C4Entity.Pickup( _c4.NetworkIdent );
	}

	public void Destroy()
	{
		C4Entity.DeleteC4( _c4.NetworkIdent );
	}
}
