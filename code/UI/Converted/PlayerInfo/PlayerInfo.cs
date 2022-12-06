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
		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( player.CurrentPlayer.IsValid() && player.CurrentPlayer.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		var hasRole = player.CurrentPlayer.Role is not NoneRole;
		HealthContainer.SetClass( "left-flat", hasRole );
		RoleContainer.Enabled( hasRole );

		if ( hasRole )
		{
			RoleContainer.Style.BackgroundColor = player.CurrentPlayer.Role.Color;
			Role.Text = player.CurrentPlayer.Role.Title;
		}

		Health.Text = $"✚ {player.CurrentPlayer.Health.CeilToInt()}";
	}

	[GameEvent.Player.TookDamage]
	private void OnHit( Player player )
	{
		if ( !this.IsEnabled() )
			return;

		HealthContainer.AddClass( "hit" );
		Utils.DelayAction( 0.2f, () => { HealthContainer.RemoveClass( "hit" ); } );
	}
}
