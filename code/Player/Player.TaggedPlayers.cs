using System.Collections.Generic;
using Sandbox;

namespace TTT;

public partial class Player
{
	public static readonly ColorGroup[] TagGroupList = new ColorGroup[]
	{
		new ColorGroup("Friend", Color.FromBytes(0, 255, 0)),
		new ColorGroup("Suspect", Color.FromBytes(179, 179, 20)),
		new ColorGroup("Avoid", Color.FromBytes(252, 149, 0)),
		new ColorGroup("Kill", Color.FromBytes(255, 0, 0)),
		new ColorGroup("Missing", Color.FromBytes(130, 190, 130))
	};

	/// <summary>
	/// Dictionary keying tagger entities to selected tag groups.
	/// </summary>
	public Dictionary<Entity, ColorGroup> TaggedPlayers = new();
}
