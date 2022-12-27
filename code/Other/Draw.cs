using System;
using System.Collections.Generic;
using Sandbox;

namespace TTT;

public static class Draw
{
	public static List<Vertex> VertexList { get; } = new();

	public static void AddVertex( in Vertex position )
	{
		VertexList.Add( position );
	}

	public static void AddVertex( in Vector2 position, in Color color )
	{
		var vertex = new Vertex
		{
			Position = position,
			Color = color
		};

		AddVertex( in vertex );
	}

	public static void AddVertex( in Vector2 position, in Color color, in Vector2 uv )
	{
		var vertex = new Vertex
		{
			Position = position,
			Color = color,
			TexCoord0 = uv
		};

		AddVertex( in vertex );
	}

	public static void MeshStart()
	{
		VertexList.Clear();
	}

	public static void MeshEnd( RenderAttributes _attributes = null )
	{
		var attributes = _attributes ?? new RenderAttributes();

		attributes.Set( "Texture", Texture.White );

		Graphics.Draw( VertexList.ToArray(), VertexList.Count, Material.UI.Basic, attributes );
		VertexList.Clear();
	}

	public static void Circle( in Vector2 center, in Color color, float radius, int points = 32 )
	{
		Circle( in center, radius, 0f, in color, points );
	}

	public static void Ring( in Vector2 center, float radius, in Color color, float holeRadius, int points = 32 )
	{
		Circle( in center, radius, holeRadius, in color, points );
	}

	public static void Circle( in Vector2 center, float outerRadius, float innerRadius, in Color color, int pointCount = 32, float startAngle = 0f, float endAngle = 360f, float uv = 0f )
	{
		MeshStart();

		var twoPI = MathF.PI * 2f;
		startAngle = startAngle.NormalizeDegrees().DegreeToRadian();

		for ( endAngle = endAngle.NormalizeDegrees().DegreeToRadian(); endAngle <= startAngle; endAngle += twoPI ) ;

		if ( endAngle <= startAngle )
			return;

		var angleStep = twoPI / pointCount;

		for ( var currentAngle = startAngle; currentAngle < endAngle + angleStep; currentAngle += angleStep )
		{
			var startRadians = currentAngle;
			var endRadians = currentAngle + angleStep;

			if ( endRadians > endAngle )
			{
				endRadians = endAngle;
			}

			startRadians += MathF.PI;
			endRadians += MathF.PI;

			var startVector = new Vector2( MathF.Sin( -startRadians ), MathF.Cos( -startRadians ) );
			var endVector = new Vector2( MathF.Sin( -endRadians ), MathF.Cos( -endRadians ) );
			var startUV = startVector / 2f + 0.5f;
			var endUV = endVector / 2f + 0.5f;
			var innerStartUV = startVector * innerRadius / outerRadius / 2f + 0.5f;
			var innerEndUV = endVector * innerRadius / outerRadius / 2f + 0.5f;

			if ( uv > 0f )
			{
				startUV = new Vector2( (startRadians - MathF.PI - startAngle) * uv / twoPI, 0f );
				endUV = new Vector2( (endRadians - MathF.PI - startAngle) * uv / twoPI, 0f );
				innerStartUV = startUV.WithY( 1f );
				innerEndUV = endUV.WithY( 1f );
			}

			var v = center + startVector * outerRadius;
			AddVertex( in v, in color, in startUV );

			v = center + endVector * outerRadius;
			AddVertex( in v, in color, in endUV );

			v = center + startVector * innerRadius;
			AddVertex( in v, in color, in innerStartUV );

			if ( innerRadius > 0f )
			{
				v = center + endVector * outerRadius;
				AddVertex( in v, in color, in endUV );

				v = center + endVector * innerRadius;
				AddVertex( in v, in color, in innerEndUV );

				v = center + startVector * innerRadius;
				AddVertex( in v, in color, in innerStartUV );
			}
		}

		MeshEnd();
	}

	public static void Line( in Color color, in float startThickness, in Vector2 startPosition, in float endThickness, in Vector2 endPosition )
	{
		MeshStart();

		var directionVector = endPosition - startPosition;
		var perpendicularVector = directionVector.Perpendicular.Normal * -0.5f;

		var startCorner = startPosition + perpendicularVector * startThickness;
		var endCorner = startPosition + directionVector + perpendicularVector * endThickness;
		var endCorner2 = startPosition + directionVector - perpendicularVector * endThickness;
		var startCorner2 = startPosition - perpendicularVector * startThickness;

		var uv = new Vector2( 0f, 0f );
		AddVertex( in startCorner, in color, in uv );

		uv = new Vector2( 1f, 0f );
		AddVertex( in endCorner, in color, in uv );

		uv = new Vector2( 0f, 1f );
		AddVertex( in startCorner2, in color, in uv );

		uv = new Vector2( 1f, 0f );
		AddVertex( in endCorner, in color, in uv );

		uv = new Vector2( 1f, 1f );
		AddVertex( in endCorner2, in color, in uv );

		uv = new Vector2( 0f, 1f );
		AddVertex( in startCorner2, in color, in uv );

		MeshEnd();
	}

	public static void Line( in Color color, in float thickness, in Vector2 startPosition, in Vector2 endPosition )
	{
		Line( in color, in thickness, in startPosition, in thickness, in endPosition );
	}
}
