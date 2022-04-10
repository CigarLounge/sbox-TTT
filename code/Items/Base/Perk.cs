using Sandbox;

namespace TTT.Items;

[Library( "perk" ), AutoGenerate]
public partial class PerkInfo : ItemInfo
{

}

public abstract class Perk : EntityComponent<Player>
{
	public virtual string SlotText => string.Empty;
	public PerkInfo Info { get; private set; }

	public Perk()
	{
		Info = Asset.GetInfo<PerkInfo>( this );
	}

	public virtual void Simulate( Client client ) { }

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = Asset.GetInfo<PerkInfo>( this );
	}
#endif
}
