using Sandbox;
using System.Collections.Generic;

namespace TTT.Items;

[Library( "ttt_perk_radar", Title = "Radar" )]
public partial class Radar : Perk
{
	[Net, Local]
	private TimeUntil TimeUntilExecution { get; set; }

	public override string SlotText => TimeUntilExecution.Relative.CeilToInt().ToString();
	private readonly float _timeToExecute = 20f;
	private RadarPointData[] _lastPositions;
	private readonly List<UI.RadarPoint> _cachedPoints = new();
	private readonly Color _defaultRadarColor = Color.FromBytes( 124, 252, 0 );
	private readonly Vector3 _radarPointOffset = Vector3.Up * 45;

	public Radar()
	{
		// We should execute as soon as the perk is equipped.
		TimeUntilExecution = 0;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient )
			UI.WorldPoints.Instance.DeletePoints<UI.RadarPoint>();
	}

	public override void Simulate( Client client )
	{
		if ( !TimeUntilExecution )
			return;

		UpdatePositions();
		TimeUntilExecution = _timeToExecute;
	}

	private void UpdatePositions()
	{
		if ( Host.IsClient )
		{
			ClearRadarPoints();

			if ( _lastPositions.IsNullOrEmpty() )
				return;

			foreach ( var radarData in _lastPositions )
				_cachedPoints.Add( new UI.RadarPoint( radarData ) );

			return;
		}

		List<RadarPointData> pointData = new();
		foreach ( var entity in Sandbox.Entity.All )
		{
			if ( entity is Player player )
			{
				if ( player.Client == Entity.Client )
					continue;

				if ( !player.IsAlive() )
					continue;

				pointData.Add( new RadarPointData
				{
					Position = player.Position + _radarPointOffset,
					Color = player.Role == Entity.Role ? Entity.Role.Info.Color : _defaultRadarColor
				} );
			}
			else if ( Entity.Team != Team.Traitors && entity is Entities.Decoy decoy )
			{
				pointData.Add( new RadarPointData
				{
					Position = decoy.Position,
					Color = _defaultRadarColor
				} );
			}
		}

		ClientSendRadarPositions( To.Single( Entity ), Entity, pointData.ToArray() );
	}

	private void ClearRadarPoints()
	{
		foreach ( UI.RadarPoint radarPoint in _cachedPoints )
			radarPoint.Delete( true );

		_cachedPoints.Clear();
	}

	[ClientRpc]
	public static void ClientSendRadarPositions( Player player, RadarPointData[] points )
	{
		if ( !player.IsValid() || !player.IsLocalPawn )
			return;

		var radar = player.Perks.Find<Radar>();
		if ( radar is null )
			return;

		radar._lastPositions = points;
		radar.UpdatePositions();
	}
}

public struct RadarPointData
{
	public Color Color;
	public Vector3 Position;
}
