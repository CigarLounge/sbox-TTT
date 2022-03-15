using Sandbox;

namespace TTT;

[Library( "perk" ), AutoGenerate]
public partial class PerkInfo : ItemInfo
{

}

public abstract class Perk : EntityComponent<Player>
{
	public virtual string ActiveText => string.Empty;
	public PerkInfo Info { get; private set; }

	// We need this because Entity is null OnDeactivate()
	private Player _entity;

	public Perk()
	{
		Info = Asset.GetInfo<PerkInfo>( this );
	}

	public virtual void Simulate( Player player ) { }

	protected override void OnActivate()
	{
		base.OnActivate();

		_entity = Entity;

		if ( Host.IsClient )
			Entity.Perks.Add( this );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient )
			_entity.Perks.Remove( this );
	}

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = Asset.GetInfo<PerkInfo>( this );
	}
#endif
}
