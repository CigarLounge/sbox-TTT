using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_decoy", Title = "Decoy" )]
public class Decoy : Deployable<DecoyEntity>
{
	protected override void OnDeploy( DecoyEntity entity )
	{
		base.OnDeploy( entity );

		var decoyComponent = PreviousOwner.Components.GetOrCreate<DecoyComponent>();
		decoyComponent.Decoy = entity;
	}
}

public partial class DecoyComponent : EntityComponent<Player>
{
	public DecoyEntity Decoy { get; set; }
}
