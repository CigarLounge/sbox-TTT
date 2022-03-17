using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_binoculars", Title = "Binoculars" )]
public partial class Binoculars : Carriable
{
	enum Zoom
	{
		None,
		One,
		Two,
		Three,
		Four
	}

	private float _defaultFOV;
	private Zoom _zoomLevel = Zoom.None;

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		_defaultFOV = Owner.CameraMode.FieldOfView;
	}


	[Event.BuildInput]
	private new void BuildInput( InputBuilder input )
	{

	}

	private void ChangeZoomLevel()
	{

	}
}
