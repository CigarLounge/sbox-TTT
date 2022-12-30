using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_radio" )]
[Title( "Radio" )]
public class Radio : Deployable<RadioEntity>
{
	protected override void OnDeploy( RadioEntity entity )
	{
		base.OnDeploy( entity );

		var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
		radioComponent.Radio = entity;
	}
}

public partial class RadioComponent : EntityComponent<Player>
{
	[Net, Local]
	public RadioEntity Radio { get; set; }
}
