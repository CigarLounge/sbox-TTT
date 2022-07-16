using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace TTT;

[GameResource( "Role", "role", "TTT role template.", Icon = "🎭" )]
public class RoleInfo : GameResource
{
	public Team Team { get; set; } = Team.None;

	[Category( "Shop" )]
	[Description( "The amount of credits the player spawns with." )]
	public int DefaultCredits { get; set; } = 0;

	[Category( "Shop" ), ResourceType( "weapon" )]
	public List<string> Weapons { get; set; } = new();

	[Category( "Shop" ), ResourceType( "carri" )]
	public List<string> Carriables { get; set; } = new();

	[Category( "Shop" ), ResourceType( "perk" )]
	public List<string> Perks { get; set; } = new();

	public bool CanRetrieveCredits { get; set; } = false;

	public bool CanTeamChat { get; set; } = false;

	public bool CanAttachCorpses { get; set; } = false;

	[Category( "UI" )]
	public Color Color { get; set; }

	[Title( "Icon" ), Category( "UI" ), ResourceType( "png" )]
	public string IconPath { get; set; } = "ui/none.png";

	[HideInEditor]
	[JsonIgnore]
	public HashSet<ItemInfo> ShopItems { get; private set; } = new();

	[HideInEditor]
	[JsonIgnore]
	public Texture Icon { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( ResourceLibrary is null )
			return;

		var itemPaths = Weapons.Concat( Carriables ).Concat( Perks );
		foreach ( var itemPath in itemPaths )
		{
			var itemInfo = ResourceLibrary.Get<ItemInfo>( itemPath );
			if ( itemInfo is null )
				continue;

			ShopItems.Add( itemInfo );
		}

		if ( Host.IsClient )
			Icon = Texture.Load( FileSystem.Mounted, IconPath );
	}
}
