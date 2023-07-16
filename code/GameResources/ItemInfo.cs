using Sandbox;
using System.Collections.Generic;

namespace TTT;

public abstract class ItemInfo : GameResource
{
	[Category( "Important" )]
	public bool IsLimited { get; set; } = false;

	[Category( "Stats" )]
	public int Price { get; set; } = 0;

	public List<Team> PurchasableBy { get; set; }

	[Title( "Icon" ), Category( "UI" ), ResourceType( "png" )]
	public string IconPath { get; set; } = "ui/none.png";

	[Category( "UI" )]
	public string Description { get; set; } = "";

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( PurchasableBy is null )
			return;

		foreach ( var team in PurchasableBy )
			TeamExtensions._shopItems[team].Add( this );
	}
}
