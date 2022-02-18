using Sandbox;

using TTT.Player;

namespace TTT.Items;

[Library("perk"), AutoGenerate]
public partial class PerkInfo : ItemInfo
{

}

public abstract class Perk : BaseNetworkable
{
	public PerkInfo Info { get; set; }
	public virtual void Simulate( TTTPlayer player ) { }
	public virtual string ActiveText() { return string.Empty; }

	public Perk()
	{
		Info = ItemInfo.All[ClassInfo.Name] as PerkInfo;
	}
}
