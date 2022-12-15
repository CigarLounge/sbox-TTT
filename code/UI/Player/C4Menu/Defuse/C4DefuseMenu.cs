using System;
using Sandbox;
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
	private Label Disclaimer { get; set; }

	public C4DefuseMenu( C4Entity c4 )
	{
		_c4 = c4;

		for ( var i = 0; i < C4Entity.Wires.Count; ++i )
		{
			var wireNumber = i + 1;
			var wire = new Wire( wireNumber, C4Entity.Wires[i] );
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

		Disclaimer.Text = (Game.LocalPawn as Player) == _c4.Owner ? "It's your C4, any wire will defuse it" : string.Empty;

		if ( _c4.IsArmed )
			TimerDisplay.Text = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
	}

	public void Defuse( int wire )
	{
		if ( _c4.IsArmed )
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
