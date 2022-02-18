using System.Collections.Generic;
using Sandbox;

namespace TTT.Player;

public partial class BaseClothing : ModelEntity
{
	public TTTPlayer Wearer => Parent as TTTPlayer;
	public virtual void Attached() { }
	public virtual void Detatched() { }
}

public partial class TTTPlayer
{
	protected List<BaseClothing> Clothing { get; set; } = new();

	public BaseClothing AttachClothing( string modelName )
	{
		var entity = new BaseClothing();
		entity.SetModel( modelName );
		AttachClothing( entity );
		return entity;
	}

	public T AttachClothing<T>() where T : BaseClothing, new()
	{
		var entity = new T();
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
	}
}
