using Sandbox;
using System.Collections.Generic;
using System.ComponentModel;

namespace TTT;

public partial class Asset : Sandbox.Asset
{
	private static Dictionary<string, Asset> Collection { get; set; } = new();

	[Property( "libraryname", "The name you define in the Library Attribute in code." ), Category( "Important" )]
	public string LibraryName { get; set; }
	public string Title { get; set; }

	public static T CreateFromAssetId<T>( int id ) where T : LibraryClass
	{
		return Library.Create<T>( FromId<Asset>( id ).LibraryName );
	}

	public static T GetInfo<T>( LibraryClass libraryClass ) where T : Asset
	{
		if ( libraryClass is null || !Collection.ContainsKey( libraryClass.ClassInfo.Name ) )
			return null;

		return Collection[libraryClass.ClassInfo.Name] as T;
	}

	public static T GetInfo<T>( string libraryName ) where T : Asset
	{
		if ( string.IsNullOrEmpty( libraryName ) || !Collection.ContainsKey( libraryName ) )
			return null;

		return Collection[libraryName] as T;
	}

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( string.IsNullOrEmpty( LibraryName ) )
			return;

		var attribute = Library.GetAttribute( LibraryName );

		if ( attribute == null )
			return;

		Collection[LibraryName] = this;
		Title = attribute.Title;
	}
}
