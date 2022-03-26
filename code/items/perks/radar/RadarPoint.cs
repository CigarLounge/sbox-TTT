using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT;

public class RadarPoint : Panel
{
	private readonly Vector3 _position;
	private readonly Label _distanceLabel;
	private const int BLUR_RADIUS = 10;

	public RadarPoint( RadarPointData data )
	{
		if ( RadarDisplay.Instance == null )
			return;

		_position = data.Position;

		StyleSheet.Load( "/items/perks/radar/RadarPoint.scss" );

		RadarDisplay.Instance.AddChild( this );

		AddClass( "circular" );

		_distanceLabel = Add.Label();
		_distanceLabel.AddClass( "distance-label" );
		_distanceLabel.AddClass( "text-shadow" );

		Style.BackgroundColor = data.Color;
		Style.BoxShadow = new ShadowList()
		{
			new Shadow
			{
				Blur = BLUR_RADIUS,
				Color = data.Color
			}
		};
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		_distanceLabel.Text = $"{player.Position.Distance( _position ).SourceUnitsToMeters():n0}m";

		Vector3 screenPos = _position.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}

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
