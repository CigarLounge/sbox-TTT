using Sandbox;

namespace TTT;

public partial class WalkController
{
	[ConVar.Replicated( "debug_playercontroller" )]
	public static bool Debug { get; set; } = false;

	/// <summary>
	/// Any bbox traces we do will be offset by this amount.
	/// todo: this needs to be predicted
	/// </summary>
	public Vector3 TraceOffset;

	private Vector3 _mins;
	private Vector3 _maxs;

	public BBox GetHull()
	{
		var girth = BodyGirth * 0.5f;
		var mins = new Vector3( -girth, -girth, 0 );
		var maxs = new Vector3( +girth, +girth, BodyHeight );

		return new BBox( mins, maxs );
	}

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
							.Size( mins, maxs )
							.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
							.Ignore( Player )
							.Run();

		tr.EndPosition -= TraceOffset;
		return tr;
	}

	/// <summary>
	/// This calls TraceBBox with the right sized bbox. You should derive this in your controller if you 
	/// want to use the built in functions
	/// </summary>
	public TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
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

		var mins = new Vector3( -girth, -girth, 0 ) * Player.Scale;
		var maxs = new Vector3( +girth, +girth, BodyHeight ) * Player.Scale;

		Duck.UpdateBBox( ref mins, ref maxs, Player.Scale );

		SetBBox( mins, maxs );
	}
}
