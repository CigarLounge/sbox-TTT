using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_binoculars", Title = "Binoculars" )]
public partial class Binoculars : Carriable
{
	[Net, Predicted]
	private Corpse Corpse { get; set; }

	[Net, Predicted]
	private float ZoomLevel { get; set; }
	public bool IsZoomed => ZoomLevel > 0;

	private float _defaultFOV;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		_defaultFOV = Owner.CameraMode.FieldOfView;
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		if ( Corpse.IsValid() )
			Corpse.HintDistance = Player.INTERACT_DISTANCE;

		Corpse = null;
		ZoomLevel = 0;
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

		if ( lastCorpse != Corpse )
		{
			if ( lastCorpse.IsValid() )
				lastCorpse.HintDistance = Player.INTERACT_DISTANCE;
			if ( Corpse.IsValid() )
				Corpse.HintDistance = Player.MAX_HINT_DISTANCE;
		}

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
			input.ViewAngles = Angles.Lerp( input.OriginalViewAngles, input.ViewAngles, 0.4f / ZoomLevel );
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		(Local.Pawn as Player).CameraMode.FieldOfView = _defaultFOV;
	}

	private void ChangeZoomLevel()
	{
		if ( ZoomLevel >= 4 )
		{
			if ( Corpse.IsValid() )
				Corpse.HintDistance = Player.INTERACT_DISTANCE;

			Corpse = null;
			ZoomLevel = 0;
			Owner.CameraMode.FieldOfView = _defaultFOV;

			return;
		}

		PlaySound( RawStrings.ScopeInSound );
		ZoomLevel++;
		Owner.CameraMode.FieldOfView = 40f / ZoomLevel;
	}
}
