using Sandbox;

namespace TTT;

[Library( "perk" ), AutoGenerate]
public class PerkInfo : ItemInfo { }

public abstract class Perk : EntityComponent<Player>
{
	public virtual string SlotText => string.Empty;
	public PerkInfo Info { get; private set; }

	public Perk()
	{
		Info = Asset.GetInfo<PerkInfo>( GetType() );
	}

	public virtual void Simulate( Client client ) { }

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = Asset.GetInfo<PerkInfo>( GetType() );
	}
#endif
}
