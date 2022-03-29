using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class RadarDisplay : Panel
{
	public static RadarDisplay Instance { get; set; }

	public RadarDisplay() : base()
	{
		Instance = this;
		AddClass( "fullscreen" );
		Style.ZIndex = -1;
	}
}

[UseTemplate]
public class RadarPoint : Panel
{
	private readonly Vector3 _position;
	private Label Distance { get; set; }

	public RadarPoint( RadarPointData data )
	{
		if ( RadarDisplay.Instance is null )
			return;

		_position = data.Position;
		RadarDisplay.Instance.AddChild( this );

		Style.BackgroundColor = data.Color;
		Style.BoxShadow = new ShadowList()
		{
			new Shadow
			{
				Color = data.Color,
				Blur = 5
			}
		};
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;

		Distance.Text = $"{player.Position.Distance( _position ).SourceUnitsToMeters():n0}m";

		var screenPos = _position.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
