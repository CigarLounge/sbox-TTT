using Sandbox;

namespace TTT;

public class FirstPersonCamera : CameraMode
{
	public FirstPersonCamera( Player viewer = null )
	{
		Spectating.Player = viewer;
		Camera.FirstPersonViewer = viewer ?? Game.LocalPawn;
	}

	public override void BuildInput()
	{
		if ( Game.LocalPawn is Player player && player.IsAlive )
			return;

		if ( !Spectating.Player.IsValid() || Input.Pressed( InputButton.Jump ) )
		{
			Current = new FreeCamera();
			return;
		}

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			Spectating.FindPlayer( false );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
			Spectating.FindPlayer( true );
	}

	public override void FrameSimulate( Player player )
	{
		var target = UI.Hud.DisplayedPlayer;

		if ( !target.IsValid() )
			return;

		Camera.Position = target.EyePosition;
		Camera.Rotation = target == Spectating.Player ? Rotation.Slerp( Camera.Rotation, target.EyeLocalRotation, Time.Delta * 20f ) : target.EyeRotation;
		Camera.FirstPersonViewer = target;

		// TODO: We need some way to override the FieldOfView from a carriable.
		// We also need to constantly update the field of view here incase the player changes their settings.
		if ( target.ActiveCarriable is Scout || target.ActiveCarriable is Binoculars )
			return;

		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.Main.SetViewModelCamera( 95f );
	}
}
