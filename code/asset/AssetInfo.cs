using Sandbox;
using System.Collections.Generic;

namespace TTT;

[Hammer.Skip]
public partial class AssetInfo : Asset
{
	public static Dictionary<string, AssetInfo> Collection { get; set; } = new();
	[Property] public string LibraryName { get; set; }
	[Property] public string Title { get; set; } = "";

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
