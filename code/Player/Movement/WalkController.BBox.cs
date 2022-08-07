using Sandbox;

namespace TTT;

public partial class WalkController : BasePlayerController
{
	private Vector3 _mins;
	private Vector3 _maxs;

	public override BBox GetHull()
	{
		var girth = BodyGirth * 0.5f;
		var mins = new Vector3( -girth, -girth, 0 );
		var maxs = new Vector3( +girth, +girth, BodyHeight );

		return new BBox( mins, maxs );
	}

	/// <summary>
	/// Traces the current bbox and returns the result.
	/// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public override TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
	{
		return TraceBBox( start, end, _mins, _maxs, liftFeet );
	}

	private void SetBBox( Vector3 mins, Vector3 maxs )
	{
		if ( _mins == mins && _maxs == maxs )
			return;

		_mins = mins;
		_maxs = maxs;
	}

	private void UpdateBBox()
	{
		var girth = BodyGirth * 0.5f;

		var mins = new Vector3( -girth, -girth, 0 ) * Pawn.Scale;
		var maxs = new Vector3( +girth, +girth, BodyHeight ) * Pawn.Scale;

		Duck.UpdateBBox( ref mins, ref maxs, Pawn.Scale );

		SetBBox( mins, maxs );
	}
}
