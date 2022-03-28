using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

[Library( "ttt_perk_radar", Title = "Radar" )]
public partial class Radar : Perk
{
	public override string ActiveText => $"{Math.Abs( Math.Round( _timeUntilExecution ) )}";
	private readonly float _timeToExecute = 20f;
	private TimeUntil _timeUntilExecution;
	private RadarPointData[] _lastPositions;
	private readonly List<RadarPoint> _cachedPoints = new();
	private readonly Color _defaultRadarColor = Color.FromBytes( 124, 252, 0 );
	private readonly Vector3 _radarPointOffset = Vector3.Up * 45;

	public Radar()
	{
		// We should execute as soon as the perk is equipped.
		_timeUntilExecution = 0;
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Host.IsClient )
			Local.Hud.AddChild( new RadarDisplay() );
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Host.IsClient )
			RadarDisplay.Instance?.Delete();
	}

	public override void Simulate( Player player )
	{
		if ( Math.Round( _timeUntilExecution ) < 0f )
		{
			UpdatePositions( player );
			_timeUntilExecution = _timeToExecute;
		}
	}

	private void UpdatePositions( Player owner )
	{
		if ( Host.IsClient )
		{
			ClearRadarPoints();

			if ( _lastPositions.IsNullOrEmpty() )
				return;

			foreach ( var radarData in _lastPositions )
				_cachedPoints.Add( new RadarPoint( radarData ) );

			return;
		}


		List<RadarPointData> pointData = new();
		foreach ( var ent in Sandbox.Entity.All )
		{
			if ( ent is Player player )
			{
				if ( player.Client == owner.Client )
					continue;

				pointData.Add( new RadarPointData
				{
					Position = player.Position + _radarPointOffset,
					Color = player.Role == owner.Role ? owner.Role.Info.Color : _defaultRadarColor
				} );
			}
			else if ( owner.Team != Team.Traitors && ent is DecoyEntity decoy )
			{
				pointData.Add( new RadarPointData
				{
					Position = decoy.Position,
					Color = _defaultRadarColor
				} );
			}
		}

		ClientSendRadarPositions( To.Single( owner ), owner, pointData.ToArray() );
	}

	private void ClearRadarPoints()
	{
		foreach ( RadarPoint radarPoint in _cachedPoints )
			radarPoint.Delete();

		_cachedPoints.Clear();
	}

	[ClientRpc]
	public static void ClientSendRadarPositions( Player player, RadarPointData[] points )
	{
		if ( !player.IsValid() || player != Local.Pawn )
			return;

		Radar radar = player.Perks.Find<Radar>();
		if ( radar == null )
			return;

		radar._lastPositions = points;
		radar.UpdatePositions( player );
	}
}

public struct RadarPointData
{
	public Color Color;
	public Vector3 Position;
}
