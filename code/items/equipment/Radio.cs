using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_radio", Title = "Radio" )]
public partial class Radio : Carriable
{
	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			var droppedRadio = Owner.Inventory.DropEntity( this, typeof( RadioEntity ) );
			if ( droppedRadio is null || droppedRadio is not RadioEntity radio )
				return;

			radio.RadioOwner = PreviousOwner;
			var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
			radioComponent.Radio = radio;
		}
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
