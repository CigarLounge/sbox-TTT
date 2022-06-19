using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_decoy" )]
[HideInEditor]
[Title( "Decoy" )]
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
