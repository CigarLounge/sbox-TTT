using Sandbox;

namespace TTT;

public class FirstPersonSpectatorCamera : CameraMode, ISpectateCamera
{
	private Player _owner;
	private const float SmoothSpeed = 25f;

	protected override void OnActivate()
	{
		_owner = Entity as Player;
		_owner.UpdateSpectatedPlayer();
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

	public override void BuildInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.PrimaryAttack ) )
			_owner.UpdateSpectatedPlayer( 1 );
		else if ( input.Pressed( InputButton.SecondaryAttack ) )
			_owner.UpdateSpectatedPlayer( -1 );

		base.BuildInput( input );
	}

	public void OnUpdateSpectatedPlayer( Player newSpectatedPlayer )
	{
		Viewer = newSpectatedPlayer;
	}
}
