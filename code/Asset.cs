using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TTT;

public abstract class Asset : GameResource
{
	private static readonly Dictionary<string, Asset> Collection = new( StringComparer.OrdinalIgnoreCase );

	[Property, Category( "Important" )]
	public string LibraryName { get; set; }

	public string Title { get; private set; }

	public static T GetInfo<T>( Type type ) where T : Asset
	{
		if ( type is null )
			return null;

		return GetInfo<T>( TypeLibrary.GetDescription( type ).ClassName );
	}

	public static T GetInfo<T>( string className ) where T : Asset
	{
		if ( string.IsNullOrEmpty( className ) || !Collection.ContainsKey( className ) )
			return null;

		return Collection[className] as T;
	}

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( string.IsNullOrWhiteSpace( LibraryName ) )
			return;

		var description = TypeLibrary.GetDescription<object>( LibraryName );

		if ( description is null )
			return;

		Title = description.Title;
		Collection[LibraryName] = this;

		if ( !string.IsNullOrWhiteSpace( Title ) )
			Collection[Title] = this;
	}
}
