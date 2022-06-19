using System;
using System.Collections.Generic;

namespace TTT;

public abstract class GameResource : Sandbox.GameResource
{
	private static readonly Dictionary<string, GameResource> _collection = new( StringComparer.OrdinalIgnoreCase );

	[Category( "Important" )]
	public string ClassName { get; set; }

	[HideInEditor]
	public string Title { get; set; }

	public static T GetInfo<T>( Type type ) where T : GameResource
	{
		if ( type is null )
			return null;

		return GetInfo<T>( TypeLibrary.GetDescription( type ).ClassName );
	}

	public static T GetInfo<T>( string className ) where T : GameResource
	{
		if ( string.IsNullOrEmpty( className ) || !_collection.ContainsKey( className ) )
			return null;

		return _collection[className] as T;
	}

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( TypeLibrary is null )
			return;

		if ( string.IsNullOrWhiteSpace( ClassName ) )
			return;

		if ( _collection.ContainsKey( ClassName ) )
		{
			Log.Error( $"There is already a resource tied to {ClassName}" );
			return;
		}

		var typeDescription = TypeLibrary.GetDescription<object>( ClassName );
		if ( typeDescription is null )
			return;

		Title = typeDescription.Title;
		_collection[ClassName] = this;

		if ( !string.IsNullOrWhiteSpace( Title ) )
			_collection[Title] = this;
	}
}
