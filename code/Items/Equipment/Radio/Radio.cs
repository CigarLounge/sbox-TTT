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

	protected override void OnActivate()
	{
		if ( Entity.IsLocalPawn )
			UI.InfoFeed.AddEntry( "Radio deployed, access it using the Role Menu." );
	}

	protected override void OnDeactivate()
	{
		if ( Entity.IsLocalPawn && Entity.Inventory.Find<Radio>() is null )
			UI.InfoFeed.AddEntry( "Your radio has been destroyed." );
	}
}
