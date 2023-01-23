using System;
using System.Collections.Generic;
using Sandbox.UI;

namespace TTT.UI;

public struct ColorGroup
{
	public Color Color;
	public string Title;

	public ColorGroup( string title, Color color )
	{
		Title = title;
		Color = color;
	}
}

public partial class Nameplate : Panel
{
	public readonly Player _player;
	public Nameplate( Player player ) => _player = player;

	private static readonly List<ColorGroup> _healthGroupList = new()
	{
		new ColorGroup( "Healthy", Color.FromBytes( 44, 233, 44 ) ),
		new ColorGroup( "Hurt", Color.FromBytes( 171, 231, 3 ) ),
		new ColorGroup( "Wounded", Color.FromBytes( 213, 202, 4 ) ),
		new ColorGroup( "Badly Wounded", Color.FromBytes( 234, 129, 4 ) ),
		new ColorGroup( "Near Death", Color.FromBytes( 246, 6, 6 ) )
	};

	public static ColorGroup GetHealthGroup( Player player )
	{
		if ( player.Health > Player.MaxHealth * 0.9 )
			return _healthGroupList[0];
		else if ( player.Health > Player.MaxHealth * 0.7 )
			return _healthGroupList[1];
		else if ( player.Health > Player.MaxHealth * 0.45 )
			return _healthGroupList[2];
		else if ( player.Health > Player.MaxHealth * 0.2 )
			return _healthGroupList[3];
		else
			return _healthGroupList[4];
	}

	private static readonly List<ColorGroup> _karmaGroupList = new()
	{
		new ColorGroup("Reputable", Color.FromBytes(255, 255, 255)),
		new ColorGroup("Crude", Color.FromBytes(255, 240, 135)),
		new ColorGroup("Trigger-happy", Color.FromBytes(245, 220, 60)),
		new ColorGroup("Dangerous", Color.FromBytes(255, 180, 0)),
		new ColorGroup("Liability", Color.FromBytes(255, 130, 0))
	};

	public static ColorGroup GetKarmaGroup( Player player )
	{
		if ( player.BaseKarma > Karma.MaxValue * 0.89 )
			return _karmaGroupList[0];
		else if ( player.BaseKarma > Karma.MaxValue * 0.8 )
			return _karmaGroupList[1];
		else if ( player.BaseKarma > Karma.MaxValue * 0.65 )
			return _karmaGroupList[2];
		else if ( player.BaseKarma > Karma.MaxValue * 0.5 )
			return _karmaGroupList[3];
		else
			return _karmaGroupList[4];
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Karma.Enabled, _player.SteamName, _player.Health, _player.BaseKarma, _player.Role.GetHashCode(), _player.TagGroup.GetHashCode() );
	}
}
