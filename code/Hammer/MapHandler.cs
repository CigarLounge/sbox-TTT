using Sandbox;

namespace TTT;

public static class MapHandler
{
	public static int WeaponCount = 0;

	[Event.Entity.PostSpawn]
	public static void EntityPostSpawn()
	{
		if ( Host.IsClient )
			return;

		foreach ( var ent in Entity.All )
		{
			if ( ent is Weapon || ent is Ammo || ent is RandomWeapon )
				WeaponCount += 1;
		}
	}

	[GameEvent.Round.Started]
	private static void CleanUp()
	{
		Map.Reset( Game.DefaultCleanupFilter );
		Sandbox.Internal.Decals.RemoveFromWorld();
	}
}
