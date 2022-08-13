using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
    public static List<Clothing> ClothingPreset { get; private set; } = new();
    public ClothingContainer ClothingContainer { get; private init; } = new();

    public void DressPlayer()
    {
        if ( Game.AvatarClothing )
            ClothingContainer.LoadFromClient( Client );
        else
        {
            ClothingContainer.Clothing.Clear();

            foreach ( var clothing in ClothingPreset )
                ClothingContainer.Toggle( clothing );
        }

        ClothingContainer.DressEntity( this );
    }
}
