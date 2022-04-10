using System;
using Sandbox.UI;
using TTT.Entities;

namespace TTT.UI;

[UseTemplate]
public class C4DefuseMenu : EntityHintPanel
{
	private readonly C4 _c4;

	private Label TimerDisplay { get; set; }
	private Panel Wires { get; set; }
	private Button PickupBtn { get; set; }
	private Button DestroyBtn { get; set; }

	public C4DefuseMenu( C4 c4 )
	{
		_c4 = c4;

		for ( int i = 0; i < C4.Wires.Count; ++i )
		{
			var wireNumber = i + 1;
			var wire = new Wire( wireNumber, C4.Wires[i] );
			wire.AddEventListener( "onclick", () =>
			{
				wire.Cut();
				Defuse( wireNumber );
			} );

			Wires.AddChild( wire );
		}
	}

	public override void Tick()
	{
		PickupBtn.SetClass( "inactive", _c4.IsArmed );
		DestroyBtn.SetClass( "inactive", _c4.IsArmed );

		if ( _c4.IsArmed )
			TimerDisplay.Text = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
	}

	public void Defuse( int wire )
	{
		if ( _c4.IsArmed )
			C4.Defuse( wire, _c4.NetworkIdent );
	}

	public void Pickup()
	{
		C4.Pickup( _c4.NetworkIdent );
	}

	public void Destroy()
	{
		C4.DeleteC4( _c4.NetworkIdent );
	}
}
