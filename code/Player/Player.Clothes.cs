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

	public void DressPlayer()
	{
		ClothingContainer.Clothing = _currentPreset;

		ClothingContainer.DressEntity( this );
	}

	[GameEvent.Entity.PostSpawn]
	[GameEvent.Entity.PostCleanup]
	private static void ChangeClothingPreset()
	{
		_currentPreset = new( Game.Random.FromList( ClothingPresets ) );
	}
}
