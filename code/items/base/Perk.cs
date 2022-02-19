using Sandbox;

namespace TTT;

[Library( "perk" ), AutoGenerate]
public partial class PerkInfo : ItemInfo
{

}

public abstract class Perk : BaseNetworkable
{
	public PerkInfo Info { get; set; }
	public virtual void Simulate( Player player ) { }
	public virtual string ActiveText() { return string.Empty; }

	public Perk()
	{
		Info = AssetInfo.Collection[ClassInfo.Name] as PerkInfo;
	}
}
