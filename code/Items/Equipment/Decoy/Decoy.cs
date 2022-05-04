using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_decoy", Title = "Decoy" )]
public class Decoy : Deployable<DecoyEntity>
{
	protected override void OnDrop( Entity entity )
	{
		base.OnDrop( entity );

		var decoyComponent = PreviousOwner.Components.GetOrCreate<DecoyComponent>();
		decoyComponent.Decoy = entity as DecoyEntity;
	}
}

public partial class DecoyComponent : EntityComponent<Player>
{
	[Net, Local]
	public DecoyEntity Decoy { get; set; }
}
