using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public static List<List<Clothing>> ClothingPresets { get; private set; } = new();
	public ClothingContainer ClothingContainer { get; private init; } = new();

	private static List<Clothing> _currentPreset;

	public void DressPlayer()
	{
		// TODO: Don't load from client again if it's already loaded
		if ( Game.AvatarClothing )
			ClothingContainer.LoadFromClient( Client );
		else
		{
			ClothingContainer.Clothing.Clear();

			foreach ( var clothing in _currentPreset )
				ClothingContainer.Toggle( clothing );
		}

		ClothingContainer.DressEntity( this );
	}

	[Event.Entity.PostSpawn]
	[Event.Entity.PostCleanup]
	private static void ChangeClothingPreset()
	{
		_currentPreset = Rand.FromList( ClothingPresets );
	}
}
