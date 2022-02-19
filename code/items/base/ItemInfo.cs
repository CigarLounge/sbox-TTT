using Sandbox;
using System.ComponentModel;

namespace TTT;

[Hammer.Skip]
public abstract partial class ItemInfo : Asset
{
	public Model CachedWorldModel { get; set; }
	[Property, Category( "Important" )] public bool Buyable { get; set; }
	[Property, Category( "Stats" )] public int Price { get; set; }
	[Property, Category( "UI" ), ResourceType( "png" )] public string Icon { get; set; } = "";
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string WorldModel { get; set; } = "";

	protected override void PostLoad()
	{
		base.PostLoad();

		CachedWorldModel = Model.Load( WorldModel );
	}
}
