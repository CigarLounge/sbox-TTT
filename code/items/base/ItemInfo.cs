﻿using Sandbox;
using System.Collections.Generic;
using System.ComponentModel;

namespace TTT.Items;

[Hammer.Skip]
public abstract partial class ItemInfo : Asset
{
	public Model CachedWorldModel { get; set; }

	public static Dictionary<string, ItemInfo> All { get; set; } = new();
	[Property, Category( "Important" )] public string LibraryName { get; set; }
	[Property, Category( "Important" )] public string Title { get; set; }
	[Property, Category( "UI" ), ResourceType( "png" )] public string Icon { get; set; } = "";
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string WorldModel { get; set; } = "";

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( string.IsNullOrEmpty( LibraryName ) )
			return;

		var attribute = Library.GetAttribute( LibraryName );

		if ( attribute == null )
			return;

		All[LibraryName] = this;
		CachedWorldModel = Model.Load( WorldModel );
	}
}
