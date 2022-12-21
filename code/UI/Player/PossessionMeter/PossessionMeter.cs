using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PossessionMeter : Panel
{
	private readonly PropPossession _possession;

	public PossessionMeter( PropPossession possession )
	{
		_possession = possession;
		Game.RootPanel.AddChild( this );
	}

	protected override int BuildHash() => HashCode.Combine( _possession.Punches, _possession.MaxPunches );
}
