using Sandbox;
using System.Linq;

namespace TTT;

public class GhostEntity : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		EnableShadowCasting = false;
		Transmit = TransmitType.Never;
	}

	public void SetEntity( string model )
	{
		SetModel( model );

		RenderColor = RenderColor.WithAlpha( 0.5f );

		EnableTouch = true;
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	public void ShowValid()
	{
		RenderColor = Color.Green;
		//GlowColor = Color.Green;
	}

	public void ShowInvalid()
	{
		RenderColor = Color.Red;
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
