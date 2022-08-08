using Sandbox;

namespace TTT;

public partial class WalkController
{
	private bool _isTouchingLadder = false;
	private Vector3 _ladderNormal;

	private void CheckLadder()
	{
		var wishvel = new Vector3( Input.Forward, Input.Left, 0 );
		wishvel *= Input.Rotation.Angles().WithPitch( 0 ).ToRotation();
		wishvel = wishvel.Normal;

		if ( _isTouchingLadder )
		{
			if ( Input.Pressed( InputButton.Jump ) )
			{
				Velocity = _ladderNormal * 100.0f;
				_isTouchingLadder = false;

				return;

			}
			else if ( GroundEntity != null && _ladderNormal.Dot( wishvel ) > 0 )
			{
				_isTouchingLadder = false;
				return;
			}
		}

		const float ladderDistance = 1.0f;
		var start = Position;
		var end = start + (_isTouchingLadder ? (_ladderNormal * -1.0f) : wishvel) * ladderDistance;

		var pm = Trace.Ray( start, end )
					.Size( _mins, _maxs )
					.WithTag( "ladder" )
					.Ignore( Pawn )
					.Run();

		_isTouchingLadder = false;

		if ( pm.Hit )
		{
			_isTouchingLadder = true;
			_ladderNormal = pm.Normal;
		}
	}

	private void LadderMove()
	{
		var velocity = WishVelocity;
		var normalDot = velocity.Dot( _ladderNormal );
		var cross = _ladderNormal * normalDot;
		Velocity = velocity - cross + (-normalDot * _ladderNormal.Cross( Vector3.Up.Cross( _ladderNormal ).Normal ));

		Move();
	}
}
