using Sandbox;

namespace TTT;

public static class MapHandler
{
	public static int WeaponCount = 0;

	[GameEvent.Entity.PostSpawn]
	public static void EntityPostSpawn()
	{
		if ( Game.IsClient )
			return;

		foreach ( var ent in Entity.All )
		{
			if ( ent is Weapon || ent is Ammo || ent is RandomWeapon )
				WeaponCount += 1;
		}
	}

	public static void Cleanup()
	{
		Game.ResetMap( System.Array.Empty<Entity>() );
		Decal.Clear( true, true );
	}
}
