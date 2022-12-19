using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class DetectiveMarker : Panel
{
	public readonly Vector3 CorpseLocation;
	private string _distance;
	private Vector3 _screenPos;
	private TimeSince _timeSinceCreation;

	public DetectiveMarker( Vector3 corpseLocation )
	{
		_timeSinceCreation = 0;
		CorpseLocation = corpseLocation;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		if ( _timeSinceCreation >= 30 )
		{
			Delete();
			return;
		}

		_distance = $"{(Game.LocalPawn as Player).Position.Distance( CorpseLocation ).SourceUnitsToMeters():n0}m";
		_screenPos = CorpseLocation.ToScreen();
	}

	protected override int BuildHash() => HashCode.Combine( _distance, _screenPos );

	[GameEvent.Player.Killed]
	private void OnPlayerKilled( Player player )
	{
		if ( player.IsLocalPawn )
			Delete();
	}
}
