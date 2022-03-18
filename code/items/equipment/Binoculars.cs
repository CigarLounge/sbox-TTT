using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_binoculars", Title = "Binoculars" )]
public partial class Binoculars : Carriable
{
	[Net, Predicted]
	public Corpse Corpse { get; set; }

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

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		_defaultFOV = Owner.CameraMode.FieldOfView;
		_zoomLevel = Zoom.None;
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		IsZoomed = false;
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
				.HitLayer( CollisionLayer.Debris )
				.Run();

		var lastCorpse = Corpse;
		Corpse = trace.Entity as Corpse;

		if ( lastCorpse is not null )
			lastCorpse.HintDistance = Player.INTERACT_DISTANCE;
		if ( Corpse is not null )
			Corpse.HintDistance = Player.MAX_HINT_DISTANCE;

		if ( !IsServer || !Corpse.IsValid() )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) && !Corpse.DeadPlayer.IsConfirmedDead )
		{
			Corpse.Search( Owner, false );

			if ( !Input.Down( InputButton.Run ) )
			{
				Corpse.DeadPlayer.Confirmer = Owner;
				Corpse.DeadPlayer.Confirm();
			}
		}
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		if ( IsZoomed )
			input.ViewAngles = Angles.Lerp( input.OriginalViewAngles, input.ViewAngles, 0.4f / (float)_zoomLevel );
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		(Local.Pawn as Player).CameraMode.FieldOfView = _defaultFOV;
	}

	private void ChangeZoomLevel()
	{
		if ( _zoomLevel >= Zoom.Four )
		{
			IsZoomed = false;
			_zoomLevel = Zoom.None;
			Owner.CameraMode.FieldOfView = _defaultFOV;

			return;
		}

		IsZoomed = true;
		_zoomLevel++;
		Owner.CameraMode.FieldOfView = 40f / (float)_zoomLevel;
	}
}
