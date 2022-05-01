using Sandbox;

namespace TTT;

public class MapHandler
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
			if ( ent is Weapon || ent is Ammo || ent is RandomWeapon )
			{
				RandomWeaponCount += 1;
			}
		}
	}

	[TTTEvent.Round.Started]
	private static void CleanUp()
	{
		Map.Reset( Game.DefaultCleanupFilter );
		Sandbox.Internal.Decals.RemoveFromWorld();
	}
}
