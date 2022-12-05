using Sandbox;

namespace TTT;

public class FirstPersonSpectatorCamera : CameraMode, ISpectateCamera
{
	private Player _owner;
	private const float SmoothSpeed = 25f;

	protected override void OnActivate()
	{
		_owner = Entity as Player;
		Viewer = _owner.CurrentPlayer;
	}

	public override void Deactivated()
	{
		_owner.CurrentPlayer = null;
	}

	public override void Update()
	{
		if ( !_owner.IsSpectatingPlayer )
			return;

		Position = Vector3.Lerp( Position, Viewer.EyePosition, SmoothSpeed * Time.Delta );
		Rotation = Rotation.Slerp( Rotation, Viewer.EyeRotation, SmoothSpeed * Time.Delta );
	}

	public override void BuildInput()
	{
		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			_owner.UpdateSpectatedPlayer( 1 );
		else if ( Input.Pressed( InputButton.SecondaryAttack ) )
			_owner.UpdateSpectatedPlayer( -1 );

		base.BuildInput();
	}

	public void OnUpdateSpectatedPlayer( Player newSpectatedPlayer )
	{
		Viewer = newSpectatedPlayer;
	}
}
