using Sandbox;
using Sandbox.UI;
using System.Threading.Tasks;

namespace TTT.UI;

[UseTemplate]
public class PlayerInfo : Panel
{
	public static PlayerInfo Instance;

	private Panel RoleContainer { get; set; }
	private Label Role { get; set; }

	private Panel HealthContainer { get; set; }
	private Label Health { get; set; }

	public PlayerInfo()
	{
		Instance = this;
	}

	[GameEvent.Player.TookDamage]
	private void OnHit( Player player )
	{
		if ( !this.IsEnabled() )
			return;

		_ = TakeHit();
	}


	private async Task TakeHit()
	{
		AddClass( "hit" );
		await GameTask.Delay( 200 );
		RemoveClass( "hit" );
	}

	public override void Tick()
	{
		var player = Game.LocalPawn as Player;

		this.Enabled( PlayerCamera.Target.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		if ( PlayerCamera.Target.Role is NoneRole )
		{
			RoleContainer.Enabled( false );
			HealthContainer.Style.BorderTopLeftRadius = 4;
			HealthContainer.Style.BorderBottomLeftRadius = 4;
		}
		else
		{
			RoleContainer.Enabled( true );
			RoleContainer.Style.BackgroundColor = PlayerCamera.Target.Role.Color;
			Role.Text = PlayerCamera.Target.Role.Title;

			HealthContainer.Style.BorderTopLeftRadius = 0;
			HealthContainer.Style.BorderBottomLeftRadius = 0;
		}

		Health.Text = $"✚ {PlayerCamera.Target.Health.CeilToInt()}";
	}
}
