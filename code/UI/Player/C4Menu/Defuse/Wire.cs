using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Wire : Panel
{
	public C4Entity C4 { get; set; }
	public int Number { get; set; }
	public Color Color { get; set; }
	private bool IsCut { get; set; }

	protected override int BuildHash() => HashCode.Combine( IsCut );

	private void Defuse()
	{
		if ( C4.IsValid() && C4.IsArmed )
		{
			IsCut = true;
			C4Entity.Defuse( Number, C4.NetworkIdent );
		}
	}
}
