using System;
using System.Collections.Generic;

using Sandbox;

namespace TTT.Items;

[Library( "ttt_ammo_random" )]
public class TTTAmmoRandom : Entity
{
	public void Activate()
	{
		var ammoTypes = Library.GetAll<Ammo>();

		if ( ammoTypes. <= 0 )
		{
			return;
		}

		Type typeToSpawn = ammoTypes[Utils.RNG.Next( ammoTypes.Count )];
		TTTAmmo ent = Utils.GetObjectByType<TTTAmmo>( typeToSpawn );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Spawn();
	}
}
