using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Globals;
using TTT.Items;
using TTT.Player;

namespace TTT.Map;

public partial class MapHandler
{
	public MapSettings MapSettings { get; private set; }

	public void CleanUp()
	{
		if ( Host.IsServer )
		{
			Sandbox.Internal.Decals.RemoveFromWorld();
			EntityManager.CleanUpMap( Filter );
		}
		else
		{
			foreach ( var entity in Entity.All )
			{
				if ( entity.IsClientOnly && entity is not BaseViewModel )
					entity.Delete();
			}
		}

		return;
	}

	public static bool Filter( string className, Entity ent )
	{
		if ( className == "player" || className == "worldent" || className == "worldspawn" || className == "soundent" || className == "player_manager" )
			return false;

		// When creating entities we only have classNames to work with..
		if ( ent == null || !ent.IsValid )
			return true;

		// Gamemode related stuff, game entity, HUD, etc
		if ( ent is GameBase || ent.Parent is GameBase )
			return false;

		// Player related stuff, clothing and weapons
		foreach ( var cl in Client.All )
		{
			if ( ent == cl.Pawn || cl.Pawn.Inventory.Contains( ent ) || ent.Parent == cl.Pawn )
				return false;
		}

		return true;
	}
}
