using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class Nameplate : EntityHintPanel
{
	public readonly Player _player;
	private readonly Dictionary<Entity, ColorGroup> _tagDict;

	private Label Name { get; init; }
	private Label HealthStatus { get; init; }
	private Label KarmaStatus { get; init; }
	private Label Role { get; init; }
	private Label Tag { get; init; }

	public Nameplate( Player player ) => _player = player;

	public Nameplate(Player player, Dictionary<Entity, ColorGroup> tagDict)
	{
		_player = player;
		_tagDict = tagDict;
	}

	public override void Tick()
	{
		if ( !_player.IsValid() )
			return;

		var health = _player.Health / Player.MaxHealth * 100;
		var healthGroup = _player.GetHealthGroup( health );

		HealthStatus.Style.FontColor = healthGroup.Color;
		HealthStatus.Text = healthGroup.Title;

		var karmaGroup = Karma.GetKarmaGroup( _player );

		KarmaStatus.Style.FontColor = karmaGroup.Color;
		KarmaStatus.Text = karmaGroup.Title;

		Name.Text = _player.Client?.Name ?? "";
		if ( _player.Role is not NoneRole and not Innocent )
		{
			Role.Text = _player.Role.Title;
			Role.Style.FontColor = _player.Role.Color;
		}

		var tagGroup = GetTagGroup();
		if (tagGroup is not null)
		{
			Tag.Text = tagGroup?.Title;
			Tag.Style.FontColor = tagGroup?.Color;
		}
	}

	private ColorGroup? GetTagGroup()
	{
		try
		{
			return _tagDict[_player];
		}
		catch (Exception)
		{
			return null;
		}
	}
}
