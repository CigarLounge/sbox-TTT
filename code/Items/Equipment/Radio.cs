using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_radio", Title = "Radio" )]
public partial class Radio : Droppable<RadioEntity>
{
	protected override void OnDrop( Entity entity )
	{
		base.OnDrop( entity );

		var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
		radioComponent.Radio = entity as RadioEntity;
	}
}

public partial class RadioComponent : EntityComponent<Player>
{
	[Net, Local]
	public RadioEntity Radio { get; set; }

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsClient )
			UI.RoleMenu.Instance.AddRadioTab();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient )
			UI.RoleMenu.Instance.DeleteRadioTab();
	}
}
