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
			if ( ent is WeaponRandom )
			{
				RandomWeaponCount += 1;
			}
			else if ( ent is Weapon || ent is Ammo )
			{
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
