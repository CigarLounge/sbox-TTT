using Sandbox;

namespace TTT;

public abstract class Perk : EntityComponent<Player>
{
	public virtual string SlotText => string.Empty;
	public PerkInfo Info { get; private set; }

	public Perk()
	{
		Info = GameResource.GetInfo<PerkInfo>( GetType() );
	}

	public virtual void Simulate( Client client ) { }

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotload()
	{
		Info = GameResource.GetInfo<PerkInfo>( GetType() );
	}
#endif
}
