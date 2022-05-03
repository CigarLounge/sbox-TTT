using Sandbox;
using System.Collections.Generic;

namespace TTT;

public class BaseClothing : ModelEntity
{
	public Player Wearer => Parent as Player;
	public virtual void Attached() { }
	public virtual void Detatched() { }
}

public partial class Player
{
	protected List<BaseClothing> Clothing { get; set; } = new();

	private void DressPlayer()
	{
		AttachClothing( "models/citizen_clothes/hat/balaclava/models/balaclava.vmdl" );
		AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
		AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
		AttachClothing( "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl" );
		AttachClothing( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.vmdl" );
		AttachClothing( "models/citizen_clothes/shoes/trainers/trainers.vmdl" );

		SetClothingBodyGroups( this, 1 );
	}

	public BaseClothing AttachClothing( string modelName )
	{
		var entity = new BaseClothing();
		entity.SetModel( modelName );
		AttachClothing( entity );

		return entity;
	}

	public void AttachClothing( BaseClothing clothing )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
		clothing.Attached();

		Clothing.Add( clothing );
	}

	public void RemoveClothing()
	{
		Clothing.ForEach( ( entity ) =>
		{
			entity.Detatched();
			entity.Delete();
		} );

		Clothing.Clear();

		SetClothingBodyGroups( this, 0 );
	}

	// So that the clothes we use don't clip with the player model.
	public static void SetClothingBodyGroups( ModelEntity ent, int value )
	{
		ent.SetBodyGroup( "Chest", value );
		ent.SetBodyGroup( "Legs", value );
		ent.SetBodyGroup( "Feet", value );
	}
}
