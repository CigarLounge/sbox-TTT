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
			var radio = Owner.Inventory.DropEntity( this, new RadioEntity() ) as RadioEntity;
			var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
			radioComponent.Radio = radio;
		}
		else if ( Input.Pressed( InputButton.Attack2 ) )
		{
			var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.INTERACT_DISTANCE )
					.WorldOnly()
					.Run();

			if ( !trace.Hit )
				return;

			var radio = Owner.Inventory.DropEntity( this, new RadioEntity() ) as RadioEntity;
			var radioComponent = PreviousOwner.Components.GetOrCreate<RadioComponent>();
			radioComponent.Radio = radio;
			radio.Velocity = 0;
			radio.Position = trace.EndPosition;
			radio.Rotation = Rotation.From( trace.Normal.EulerAngles );
			radio.MoveType = MoveType.None;
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
