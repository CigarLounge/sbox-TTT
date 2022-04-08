using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4DefuseMenu : EntityHintPanel
{
	private readonly C4Entity _c4;
	private Panel Wires { get; set; }

	public C4DefuseMenu( C4Entity c4 )
	{
		_c4 = c4;

		foreach ( var wire in Wires.Children )
		{
			wire.AddEventListener( "onclick", () =>
			{
				wire.Style.Opacity = 0;
			} );
		}
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
