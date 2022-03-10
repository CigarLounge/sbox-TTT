using Sandbox;

namespace TTT;

public partial class FirstPersonSpectatorCamera : CameraMode, ISpectateCamera
{
	private const float SMOOTH_SPEED = 25f;

	public override void Activated()
	{
		base.Activated();

		if ( Local.Pawn is not Player player )
			return;

		player.UpdateSpectatedPlayer();
	}

	public override void Deactivated()
	{
		base.Deactivated();

		if ( Local.Pawn is not Player player )
			return;

		Viewer = Local.Pawn;
		player.CurrentPlayer = null;
	}

	public override void Update()
	{
		if ( Local.Pawn is not Player player )
			return;

		Position = Vector3.Lerp( Position, player.CurrentPlayer.EyePosition, SMOOTH_SPEED * Time.Delta );
		Rotation = Rotation.Slerp( Rotation, player.CurrentPlayer.EyeRotation, SMOOTH_SPEED * Time.Delta );
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( Local.Pawn is Player player )
		{
			if ( input.Pressed( InputButton.Attack1 ) )
				player.UpdateSpectatedPlayer();
		}

		base.BuildInput( input );
	}

	public void OnUpdateSpectatedPlayer( Player oldSpectatedPlayer, Player newSpectatedPlayer )
	{
		Viewer = newSpectatedPlayer;
	}
}
