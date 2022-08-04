using Sandbox;
using System.Collections.Generic;

namespace TTT;

public class Clothing : ModelEntity
{
	public override void Spawn()
	{
		Tags.Add( "trigger" );

		EnableAllCollisions = false;
		PhysicsEnabled = false;
		UsePhysicsCollision = false;
	}

	public void Detach()
	{
		Parent = null;
		EnableAllCollisions = true;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
	}
}

public partial class Player
{
	public List<Clothing> Clothes { get; protected set; } = new();

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
		var entity = new Clothing();

		entity.SetModel( path );
		AttachClothing( entity );
	}

	public bool RemoveClothing( string path )
	{
		for ( var i = Clothes.Count - 1; i >= 0; i-- )
		{
			var clothingPiece = Clothes[i];
			if ( clothingPiece.Model.ResourcePath != path )
				continue;

			clothingPiece.Delete();
			return true;
		}

		return false;
	}

	public void RemoveAllClothing()
	{
		foreach ( var clothing in Clothes.ToArray() )
			clothing.Delete();

		SetClothingBodyGroups( this, 1 );
	}

	private void AttachClothing( Clothing clothing )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
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
