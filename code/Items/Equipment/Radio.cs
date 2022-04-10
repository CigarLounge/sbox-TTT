using Sandbox;

namespace TTT.Items;

[Hammer.Skip]
[Library( "ttt_equipment_radio", Title = "Radio" )]
public partial class Radio : Droppable<Entities.Radio>
{
	protected override void OnDrop( Entity entity )
	{
		base.OnDrop( entity );

		var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
		radioComponent.Radio = entity as Entities.Radio;
	}
}

public partial class RadioComponent : EntityComponent<Player>
{
	[Net, Local]
	public Entities.Radio Radio { get; set; }

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
