using Sandbox;

namespace TTT;

public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservationCamera
{
	private const float SMOOTH_SPEED = 25f;

	public override void Deactivated()
	{
		base.Deactivated();

		if ( Local.Pawn is not Player player )
		{
			return;
		}

		Viewer = Local.Pawn;
		player.CurrentPlayer = null;
	}

	public override void Update()
	{
		if ( Local.Pawn is not Player player )
		{
			return;
		}

		if ( !player.IsSpectatingPlayer || Input.Pressed( InputButton.Attack1 ) )
		{
			player.UpdateObservatedPlayer();

			Position = player.CurrentPlayer.EyePosition;
			Rotation = player.CurrentPlayer.EyeRotation;
		}
		else
		{
			Position = Vector3.Lerp( Position, player.CurrentPlayer.EyePosition, SMOOTH_SPEED * Time.Delta );
			Rotation = Rotation.Slerp( Rotation, player.CurrentPlayer.EyeRotation, SMOOTH_SPEED * Time.Delta );
		}
	}

	public void OnUpdateObservatedPlayer( Player oldObservatedPlayer, Player newObservatedPlayer )
	{
		Viewer = newObservatedPlayer;
	}
}
