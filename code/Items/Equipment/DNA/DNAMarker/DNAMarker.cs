using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class DNAMarker : Panel
{
	private string _distance;
	private Vector3 _screenPos;
	private readonly Vector3 _targetPosition;

	public DNAMarker( Vector3 position )
	{
		_targetPosition = position;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		_distance = $"{(Game.LocalPawn as Player).Position.Distance( _targetPosition ).SourceUnitsToMeters():n0}m";
		_screenPos = _targetPosition.ToScreen();
	}

	protected override int BuildHash() => HashCode.Combine( _distance, _screenPos );
}
