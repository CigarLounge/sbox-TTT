using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_binoculars", Title = "Binoculars" )]
public partial class Binoculars : Carriable
{
	[Net, Predicted]
	public bool IsZoomed { get; private set; }

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

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( Input.Pressed( InputButton.Attack2 ) )
			ChangeZoomLevel();

		if ( !IsZoomed )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.MAX_HINT_DISTANCE )
				.Ignore( this )
				.Ignore( Owner )
				.WorldAndEntities()
				.Run();

		if ( trace.Entity is not Corpse corpse )
			return;
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );
	}

	private void ChangeZoomLevel()
	{

	}
}
