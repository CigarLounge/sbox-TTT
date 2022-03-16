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

		if ( Client.Pawn is not Player player )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			var droppedRadio = Owner.Inventory.DropEntity( this, typeof( RadioEntity ) );
			if ( droppedRadio == null )
				return;

			var radioComponent = player.Components.GetOrCreate<RadioComponent>();
			radioComponent.Radio = droppedRadio as RadioEntity;
		}
	}
}

public partial class RadioComponent : EntityComponent<Player>
{
	public RadioEntity Radio;

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
