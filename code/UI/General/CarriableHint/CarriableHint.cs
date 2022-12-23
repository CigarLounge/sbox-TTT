using System;
using Sandbox;
using Sandbox.UI;
namespace TTT.UI;

public partial class CarriableHint : Panel
{
	private string _primaryAttackHint;
	private string _secondaryAttackHint;

	public override void Tick()
	{
		var player = Game.LocalPawn as Player;
		_primaryAttackHint = player?.ActiveCarriable?.PrimaryAttackHint;
		_secondaryAttackHint = player?.ActiveCarriable?.SecondaryAttackHint;
	}

	protected override int BuildHash() => HashCode.Combine( _primaryAttackHint, _secondaryAttackHint );
}
