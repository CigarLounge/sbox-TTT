using Sandbox;
using System.Collections.Generic;
using System.ComponentModel;

namespace TTT;

public abstract partial class Asset : Sandbox.Asset
{
	private static Dictionary<string, Asset> _collection { get; set; } = new();

	[Property( "libraryname", "The name you define in the Library Attribute in code." ), Category( "Important" )]
	public string LibraryName { get; set; }
	public string Title { get; set; }

	public static T CreateFromAssetId<T>( int id ) where T : LibraryClass
	{
		return Library.Create<T>( FromId<Asset>( id ).LibraryName );
	}

	public static T GetInfo<T>( LibraryClass libraryClass ) where T : Asset
	{
		if ( libraryClass is null || !_collection.ContainsKey( libraryClass.ClassInfo.Name ) )
			return null;

		return _collection[libraryClass.ClassInfo.Name] as T;
	}

	public static T GetInfo<T>( string libraryName ) where T : Asset
	{
		if ( string.IsNullOrEmpty( libraryName ) || !_collection.ContainsKey( libraryName ) )
			return null;

		return _collection[libraryName] as T;
	}

	protected override void PostLoad()
	{
		if ( string.IsNullOrEmpty( LibraryName ) )
			return;

		var attribute = Library.GetAttribute( LibraryName );

		if ( attribute == null )
			return;

		_collection[LibraryName] = this;
		Title = attribute.Title;

		base.PostLoad();
	}
}
