using Sandbox;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TTT;

public abstract class ItemInfo : GameResource
{
	[Category( "Important" )]
	public bool Buyable { get; set; } = false;

	[Category( "Important" )]
	public bool IsLimited { get; set; } = false;

	[Category( "Stats" )]
	public int Price { get; set; } = 0;

	[JsonPropertyName( "icon" )]
	[Title( "Icon" ), Category( "UI" ), ResourceType( "png" )]
	public string IconPath { get; set; } = "";

	[Category( "UI" )]
	public string Description { get; set; } = "";

	[HideInEditor]
	[JsonPropertyName( "cached-icon" )]
	public Texture Icon { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( Host.IsClient )
			Icon = Texture.Load( FileSystem.Mounted, IconPath );
	}
}
