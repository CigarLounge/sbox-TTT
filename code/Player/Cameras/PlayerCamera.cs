using Sandbox;

namespace TTT;

public class FirstPersonCamera : CameraMode
{
	public FirstPersonCamera( Player targetPlayer ) => Target = targetPlayer;

	public override void BuildInput( Player player )
	{
		if ( !Target.IsValid() )
			player.CurrentCamera = new FreeCamera();

		if ( player.IsAlive() )
			return;

		if ( Input.Pressed( InputButton.Jump ) )
			player.CurrentCamera = new FreeCamera();

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			SwapSpectatedPlayer( false );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
			SwapSpectatedPlayer( true );
	}

	public override void FrameSimulate( Player player )
	{
		if ( !Target.IsValid() )
			return;

		Camera.Position = Target.EyePosition;
		Camera.Rotation = IsSpectatingPlayer ? Rotation.Slerp( Camera.Rotation, Target.EyeLocalRotation, Time.Delta * 20f ) : Target.EyeRotation;
		Camera.FirstPersonViewer = Target;

		// TODO: We need some way to override the FieldOfView from a carriable.
		// We also need to constantly update the field of view here incase the player changes their settings.
		if ( Target.ActiveCarriable is Scout || Target.ActiveCarriable is Binoculars )
			return;

		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.Main.SetViewModelCamera( 95f );
	}
}
