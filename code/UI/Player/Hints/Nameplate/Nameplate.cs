using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Nameplate : Panel
{
	public readonly Player _player;

	private ColorGroup _healthGroup { get; set; }
	private ColorGroup _karmaGroup { get; set; }

	public Nameplate( Player player ) => _player = player;

	public override void Tick()
	{
		if ( !_player.IsValid() )
			return;

		_healthGroup = _player.GetHealthGroup( _player.Health / Player.MaxHealth * 100 );
		_karmaGroup = Karma.GetKarmaGroup( _player );
	}

	protected override int BuildHash() => !_player.IsValid() ? -1 : HashCode.Combine( _player.Role, _player.TagGroup, _healthGroup, _karmaGroup );
}
