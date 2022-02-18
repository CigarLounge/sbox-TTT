using Sandbox;
using System.ComponentModel;
using System.Collections.Generic;

namespace TTT.gamemode;

[Hammer.Skip]
public partial class Info : Asset
{
	public static Dictionary<string, Info> All { get; set; } = new();
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

		All[LibraryName] = this;
	}
}
