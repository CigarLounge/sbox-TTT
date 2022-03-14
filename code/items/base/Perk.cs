using Sandbox;

namespace TTT;

[Library( "perk" ), AutoGenerate]
public partial class PerkInfo : ItemInfo
{

}

public abstract class Perk : BaseNetworkable
{
	public PerkInfo Info { get; init; }

	public Perk()
	{
		Info = Asset.GetInfo<PerkInfo>( this );
	}

	public virtual void Simulate( Player player ) { }
	public virtual string ActiveText() { return string.Empty; }
}
