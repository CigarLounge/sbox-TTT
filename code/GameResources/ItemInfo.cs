using Sandbox;

namespace TTT;

public abstract class ItemInfo : GameResource
{
	[Category( "Important" )]
	public bool IsLimited { get; set; } = false;

	[Category( "Stats" )]
	public int Price { get; set; } = 0;

	[Title( "Icon" ), Category( "UI" ), ResourceType( "png" )]
	public string IconPath { get; set; } = "ui/none.png";

	[Category( "UI" )]
	public string Description { get; set; } = "";
}
