using Sandbox;
using System.ComponentModel;

namespace TTT;

public abstract partial class ItemInfo : Asset
{
	[Property, Category( "Important" )] public bool Buyable { get; set; } = false;
	[Property, Category( "Important" )] public bool IsLimited { get; set; } = false;
	[Property, Category( "Stats" )] public int Price { get; set; } = 0;
	[Property, Category( "UI" ), ResourceType( "png" )] public string Icon { get; set; } = "";
	[Property, Category( "UI" )] public string Description { get; set; } = "";
}
