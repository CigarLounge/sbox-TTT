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

		foreach ( var ent in Entity.All )
		{
			if ( ent is Weapon || ent is Ammo )
			{
				// Delete all weapons and ammo since we need to wait for the assets to load.
				RandomWeaponCount += 1;
				ent.Delete();
			}
		}
	}

	public static void CleanUp()
	{
		Map.Reset( Game.DefaultCleanupFilter );
		Sandbox.Internal.Decals.RemoveFromWorld();
	}
}
