using Sandbox;
using System.Linq;

namespace TTT.Entities;

public class GhostEntity : ModelEntity
{
	public ModelEntity RealEntity { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		EnableTouch = true;
		EnableShadowCasting = false;
		EnableSolidCollisions = false;
		Transmit = TransmitType.Never;
	}

	public void SetEntity( ModelEntity entity )
	{
		RealEntity = entity;
		Model = entity.Model;

		RenderColor = RenderColor.WithAlpha( 0.5f );	
		CollisionGroup = CollisionGroup.Trigger;
		
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public void ShowValid()
	{
		RenderColor = Color.Green.WithAlpha( 0.5f ); ;
		//GlowColor = Color.Green;
	}

	public void ShowInvalid()
	{
		RenderColor = Color.Red.WithAlpha( 0.5f );
		//GlowColor = Color.Red;
	}

	public bool IsPlacementValid( ref TraceResult trace )
	{
		var position = trace.EndPosition;
		var bounds = CollisionBounds;
		var entities = Entity.FindInBox( bounds + position );
		/*
		if ( IsClient && !Fog.IsAreaSeen( position ) )
			return false;
		*/

		return entities.Count() == 0;
	}
}
