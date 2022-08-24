using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public static List<List<Clothing>> ClothingPresets { get; private set; } = new();
	public ClothingContainer ClothingContainer { get; private init; } = new();

	private static List<Clothing> _currentPreset;
	/// <summary>
	/// Cached clothes from the client owner to avoid calling <see cref="ClothingContainer.LoadFromClient(Client)"/> again.
	/// </summary>
	private List<Clothing> _avatarClothes;

	public void DressPlayer()
	{
		if ( Game.AvatarClothing )
			ClothingContainer.Clothing = _avatarClothes;
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
