using Sandbox;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TTT;

public abstract partial class ItemInfo : Asset
{
	[Property, Category( "Important" )]
	public bool Buyable { get; set; } = false;

	[Property, Category( "Important" )]
	public bool IsLimited { get; set; } = false;

	[Property, Category( "Stats" )]
	public int Price { get; set; } = 0;

	[JsonPropertyName( "icon" )]
	[Property( "icon", title: "Icon" ), Category( "UI" ), ResourceType( "png" )]
	public string IconPath { get; set; } = "";

	[Property, Category( "UI" )]
	public string Description { get; set; } = "";

	[JsonPropertyName( "cached-icon" )]
	public Texture Icon { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( Host.IsClient )
			Icon = Texture.Load( FileSystem.Mounted, IconPath );
	}
}
