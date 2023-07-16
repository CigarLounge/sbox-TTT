using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public static List<List<Clothing>> ClothingPresets { get; private set; } = new();
	public ClothingContainer ClothingContainer { get; private init; } = new();
	/// <summary>
	/// The current preset from <see cref="ClothingPresets"/>.
	/// </summary>
	private static List<Clothing> _currentPreset;
	/// <summary>
	/// Cached clothes from the client owner to avoid calling <see cref="ClothingContainer.LoadFromClient(IClient)"/> again.
	/// </summary>
	private readonly List<Clothing> _avatarClothes;

	public void DressPlayer()
	{
		ClothingContainer.Clothing = GameManager.AvatarClothing ? _avatarClothes : _currentPreset;

		ClothingContainer.DressEntity( this );
	}

	[GameEvent.Entity.PostSpawn]
	[GameEvent.Entity.PostCleanup]
	private static void ChangeClothingPreset()
	{
		_currentPreset = Game.Random.FromList( ClothingPresets );
	}
}
