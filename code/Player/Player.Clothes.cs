using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public ClothingContainer ClothingContainer { get; private init; } = new();
	private static List<Clothing> _selectedPreset = new();

	/// <summary>
	/// Cached clothes from the client owner to avoid calling <see cref="ClothingContainer.LoadFromClient(Client)"/> again.
	/// </summary>
	private readonly List<Clothing> _avatarClothes;

	/// <summary>
	/// Dresses the player with the given clothes.
	/// <para><strong>Parameters:</strong></para>
	/// <para>The clothes to dress the player with.</para>
	/// </summary>
	public void DressPlayer( List<Clothing> clothing = default )
	{
		ClothingContainer.Clothing = Game.AvatarClothing ? _avatarClothes : clothing.IsNullOrEmpty() ? _selectedPreset : clothing;
		ClothingContainer.DressEntity( this );
	}

	[Event.Entity.PostSpawn]
	[Event.Entity.PostCleanup]
	private static void ChangeClothingPreset()
	{
		_selectedPreset = Rand.FromList( GameResource.GetInfo<RoleInfo>( typeof( NoneRole ) ).ClothingSets );
	}
}
