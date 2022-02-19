using Sandbox;
using System.Collections.Generic;

namespace TTT;

[Hammer.Skip]
public partial class Asset : Sandbox.Asset
{
	private static Dictionary<string, Asset> Collection { get; set; } = new();
	[Property] public string LibraryName { get; set; }
	[Property] public string Title { get; set; } = "";

	public static T GetInfo<T>( string libraryName ) where T : Asset
	{
		if ( string.IsNullOrEmpty( libraryName ) || !Collection.ContainsKey( libraryName ) )
		{
			return null;
		}

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
	}
}
