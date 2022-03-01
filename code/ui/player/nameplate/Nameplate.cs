using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Nameplate : EntityHintPanel
{
	public Player Player;

	private readonly Panel _labelHolder;
	private readonly Label _nameLabel;
	private readonly Label _roleLabel;
	private readonly Label _damageIndicatorLabel;

	private struct HealthGroup
	{
		public string Title;
		public Color Color;
		public int MinHealth;

		public HealthGroup( string title, Color color, int minHealth )
		{
			Title = title;
			Color = color;
			MinHealth = minHealth;
		}
	}

	private HealthGroup[] HealthGroupList = new HealthGroup[]
	{
			new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 80),
			new HealthGroup("Hurt", Color.FromBytes(171, 231, 3), 60),
			new HealthGroup("Wounded", Color.FromBytes(213, 202, 4), 40),
			new HealthGroup("Badly Wounded", Color.FromBytes(234, 129, 4), 20),
			new HealthGroup("Near Death", Color.FromBytes(246, 6, 6), 0)
	};

	public Nameplate( Player player ) : base()
	{
		Player = player;

		StyleSheet.Load( "/ui/player/nameplate/Nameplate.scss" );

		_labelHolder = Add.Panel( "label-holder" );

		_nameLabel = _labelHolder.Add.Label( "", "name" );
		_nameLabel.AddClass( "text-shadow" );
		_damageIndicatorLabel = _labelHolder.Add.Label( "", "damage-indicator" );
		_damageIndicatorLabel.AddClass( "text-shadow" );
		_roleLabel = _labelHolder.Add.Label( "", "role" );
		_roleLabel.AddClass( "text-shadow-light" );

		this.Enabled( false );
	}

	private HealthGroup GetHealthGroup( float health )
	{
		foreach ( HealthGroup healthGroup in HealthGroupList )
		{
			if ( health >= healthGroup.MinHealth )
			{
				return healthGroup;
			}
		}

		return HealthGroupList[^1];
	}

	public override void UpdateHintPanel( string text )
	{
		SetClass( "disabled", !this.IsEnabled() );

		if ( !this.IsEnabled() || !Player.IsValid() )
			return;

		// Network sync workaround
		if ( Player.Health == 0 && Player.IsAlive() )
		{
			_damageIndicatorLabel.Text = "";
		}
		else
		{
			var health = Player.Health / Player.MaxHealth * 100;
			var healthGroup = GetHealthGroup( health );

			_damageIndicatorLabel.Style.FontColor = healthGroup.Color;
			_damageIndicatorLabel.Text = healthGroup.Title;
		}

		_nameLabel.Text = Player.Client?.Name ?? "";
		if ( Player.Role is not NoneRole && Player.Role is not InnocentRole )
		{
			_roleLabel.Text = Player.Role.Info.Title;
			_roleLabel.Style.FontColor = Player.Role.Info.Color;
		}
	}
}
