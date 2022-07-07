using Sandbox;
using System.Collections.Generic;

namespace TTT;

public class BaseClothing : ModelEntity
{
	public string ModelPath { get; set; }
}

public partial class Player
{
	protected List<BaseClothing> Clothing { get; set; } = new();

	private void DressPlayer()
	{
		AttachClothing( "models/citizen_clothes/hat/balaclava/models/balaclava.vmdl" );
		AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
		AttachClothing( "models/citizen_clothes/gloves/tactical_gloves/models/tactical_gloves.vmdl" );
		AttachClothing( "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl" );
		AttachClothing( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.vmdl" );
		AttachClothing( "models/citizen_clothes/shoes/trainers/trainers.vmdl" );

		SetClothingBodyGroups( this, 1 );
	}

	public void AttachClothing( string path )
	{
		var entity = new BaseClothing
		{
			ModelPath = path
		};
		entity.SetModel( path );
		AttachClothing( entity );
	}

	public void RemoveClothing( string path )
	{
		for ( var i = Clothing.Count - 1; i >= 0; i-- )
		{
			var clothingPiece = Clothing[i];
			if ( clothingPiece.ModelPath == path )
			{
				clothingPiece.Delete();
				Clothing.RemoveAt( i );
			}
		}
	}

	public void RemoveAllClothing()
	{
		foreach ( var clothing in Clothing )
			clothing.Delete();
		Clothing.Clear();

		SetClothingBodyGroups( this, 1 );
	}

	private void AttachClothing( BaseClothing clothing )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
		Clothing.Add( clothing );
	}

	// So that the clothes we use don't clip with the player model.
	public static void SetClothingBodyGroups( ModelEntity ent, int value )
	{
		ent.SetBodyGroup( "Chest", value );
		ent.SetBodyGroup( "Legs", value );
		ent.SetBodyGroup( "Feet", value );
		ent.SetBodyGroup( "Hands", value );
	}
}
