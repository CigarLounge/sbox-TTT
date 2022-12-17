using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class CorpseHint : Panel
{
	private readonly Corpse _corpse;
	public CorpseHint( Corpse corpse ) => _corpse = corpse;

	protected override int BuildHash()
	{
		return HashCode.Combine(
			_corpse,
			_corpse.CanSearch(),
			_corpse.Player.IsValid() ? _corpse.Player.IsConfirmedDead : _corpse.Player.IsValid(),
			(Game.LocalPawn as Player).Role.CanRetrieveCredits,
			(Game.LocalPawn as Player).IsAlive()
		);
	}
}
