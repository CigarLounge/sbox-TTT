using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class PlayerInfo : Panel
{
	public static PlayerInfo Instance;

	private readonly Panel _roleContainer;
	private readonly Label _role;

	private readonly Panel _healthContainer;
	private readonly Label _health;

	public PlayerInfo()
	{
		Instance = this;

		StyleSheet.Load( "/ui/player/playerinfo/PlayerInfo.scss" );

		AddClass( "opacity-heavy" );
		AddClass( "text-shadow" );

		_roleContainer = Add.Panel( "role-container" );
		_role = _roleContainer.Add.Label();

		_healthContainer = Add.Panel( "health-container background-color-primary" );
		_health = _healthContainer.Add.Label();
	}

	public void OnHit()
	{
		if ( !this.IsEnabled() )
			return;

		_ = TakeHit();
	}

	private async Task TakeHit()
	{
		AddClass( "hit" );
		await Task.Delay( 200 );
		RemoveClass( "hit" );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
		{
			this.Enabled( false );
			return;
		}

		this.Enabled( player.CurrentPlayer.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		if ( player.CurrentPlayer.Role is NoneRole )
		{
			_roleContainer.Enabled( false );
			_healthContainer.Style.BorderTopLeftRadius = 8;
			_healthContainer.Style.BorderBottomLeftRadius = 8;
		}
		else
		{
			_roleContainer.Enabled( true );
			_roleContainer.Style.BackgroundColor = player.CurrentPlayer.Role.Info.Color;
			_role.Text = player.CurrentPlayer.Role.Info.Title;

			_healthContainer.Style.BorderTopLeftRadius = 0;
			_healthContainer.Style.BorderBottomLeftRadius = 0;
		}

		_health.Text = $"✚ {player.CurrentPlayer.Health.CeilToInt()}";
	}
}