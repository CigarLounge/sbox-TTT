using System;
using System.Linq;
using Sandbox;

namespace TTT.Items;

[Library( "ttt_ammo_random" )]
public class TTTAmmoRandom : Entity
{
	public void Activate()
	{
		// TODO: Kole We can cache this.
		var ammoTypes = Library.GetAll<Ammo>().ToList();
		if ( ammoTypes.Count <= 0 )
		{
			return;
		}

		Type typeToSpawn = ammoTypes[Rand.Int( 0, ammoTypes.Count - 1 )];
		var ent = Library.Create<Ammo>( typeToSpawn );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Spawn();
	}
}
