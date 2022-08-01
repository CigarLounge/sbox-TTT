using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionNameplate : Panel
{
	private Label PlayerName { get; set; }
	private Player _player;
	private Prop _prop;

	public PossessionNameplate( Player player, Prop prop )
	{
		_player = player;
		_prop = prop;
		PlayerName.SetText( player.SteamName );
		Local.Hud.AddChild( this );
	}

	public override void Tick()
	{
		if ( _player is null || _prop is null )
		{
			Delete();
			return;
		}

		var occluded = Trace.Ray( CurrentView.Position, _prop.WorldSpaceBounds.Center )
			.WorldAndEntities()
			.Ignore( _prop )
			.Run()
			.Hit;

		if ( occluded )
		{
			Style.Opacity = 0f;
			return;
		}

		var screenPos = _prop.WorldSpaceBounds.Center.ToScreen();

		Style.Opacity = 1;
		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
