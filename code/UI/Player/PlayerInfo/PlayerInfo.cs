using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PlayerInfo : Panel
{
	private Panel RoleContainer { get; set; }
	private Label Role { get; set; }

	private Panel HealthContainer { get; set; }
	private Label Health { get; set; }

	public override void Tick()
	{
		this.Enabled( PlayerCamera.Target.IsValid() && PlayerCamera.Target.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		var hasRole = PlayerCamera.Target.Role is not NoneRole;
		HealthContainer.SetClass( "left-flat", hasRole );
		RoleContainer.Enabled( hasRole );

		if ( hasRole )
		{
			RoleContainer.Style.BackgroundColor = PlayerCamera.Target.Role.Color;
			Role.Text = PlayerCamera.Target.Role.Title;
		}

		Health.Text = $"✚ {PlayerCamera.Target.Health.CeilToInt()}";
	}

	[GameEvent.Player.TookDamage]
	private void OnHit( Player _ )
	{
		if ( !this.IsEnabled() )
			return;

		HealthContainer.AddClass( "hit" );
		Utils.DelayAction( 0.2f, () => { HealthContainer.RemoveClass( "hit" ); } );
	}
}
