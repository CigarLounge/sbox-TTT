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
		if ( Game.LocalPawn is not Player player || player.Status == PlayerStatus.Alive )
			return;

		if ( !Spectating.Player.IsValid() || Input.Pressed( InputAction.Jump ) )
		{
			Current = new FreeCamera();
			return;
		}

		if ( Input.Pressed( InputAction.PrimaryAttack ) )
			Spectating.FindPlayer( false );

		if ( Input.Pressed( InputAction.SecondaryAttack ) )
			Spectating.FindPlayer( true );
	}

	public override void FrameSimulate( IClient client )
	{
		var target = UI.Hud.DisplayedPlayer;

		Camera.Position = target.EyePosition;
		Camera.Rotation = !target.IsLocalPawn ? Rotation.Slerp( Camera.Rotation, target.EyeRotation, Time.Delta * 20f ) : target.ViewAngles.ToRotation();
		Camera.FirstPersonViewer = target;

		// TODO: We need some way to override the FieldOfView from a carriable.
		// We also need to constantly update the field of view here incase the player changes their settings.
		if ( target.ActiveCarriable is Scout || target.ActiveCarriable is Binoculars )
			return;

		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.Main.SetViewModelCamera( 95f );
	}
}
