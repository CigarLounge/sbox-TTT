using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class MapHandler
{
	public MapSettings MapSettings { get; private set; }
	public static int RandomWeaponCount = 0;

	[Event.Entity.PostSpawn]
	public static void EntityPostSpawn()
	{
		if ( Host.IsClient )
			return;

		RandomWeaponCount = Entity.All.OfType<WeaponRandom>().Count();
	}

	public static void CleanUp()
	{
		Map.Reset( Game.DefaultCleanupFilter );
		Sandbox.Internal.Decals.RemoveFromWorld();
	}
}
