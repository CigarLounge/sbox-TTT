using Sandbox;

namespace TTT;

public partial class ThirdPersonSpectateCamera : CameraMode, ISpectateCamera
{
	public Player InitialSpectatedPlayer { get; set; }

	private Player _owner;
	private Vector3 _targetPos;
	private Angles _lookAngles;

	protected override void OnActivate()
	{
		_owner = Entity as Player;

		if ( InitialSpectatedPlayer.IsValid() )
			_owner.CurrentPlayer = InitialSpectatedPlayer;
		else
			_owner.UpdateSpectatedPlayer();
	}

	public override void Deactivated()
	{
		// We don't need to remove the current player if we are swapping
		// to a FirstPersonSpectatorCamera. It should be the same player.
		if ( _owner.Camera is not FirstPersonSpectatorCamera )
			_owner.CurrentPlayer = null;
	}

	public override void Update()
	{
		if ( !_owner.IsSpectatingPlayer )
		{
			_owner.Camera = new FreeSpectateCamera();
			return;
		}

		_targetPos = Vector3.Lerp( _targetPos, _owner.CurrentPlayer.EyePosition, 50f * RealTime.Delta );

		var trace = Trace.Ray( _targetPos, _targetPos + Rotation.Forward * -120 )
			.WorldOnly()
			.Run();

		Rotation = Rotation.From( _lookAngles );
		Position = trace.EndPosition;
	}

	public override void BuildInput( InputBuilder input )
	{
		_lookAngles += input.AnalogLook;
		_lookAngles.roll = 0;

		if ( input.Pressed( InputButton.PrimaryAttack ) )
			_owner.UpdateSpectatedPlayer( 1 );
		else if ( input.Pressed( InputButton.SecondaryAttack ) )
			_owner.UpdateSpectatedPlayer( -1 );

		base.BuildInput( input );
	}
}
