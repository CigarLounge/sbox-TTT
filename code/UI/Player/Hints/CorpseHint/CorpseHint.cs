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
		var player = Game.LocalPawn as Player;
		return HashCode.Combine(
			_corpse,
			_corpse.CanSearch(),
			_corpse.Player?.IsConfirmedDead,
			_corpse.Player?.SteamName,
			player.Role.CanUseShop,
			player.IsAlive
		);
	}
}
