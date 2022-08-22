using Sandbox;
using System.Linq;

namespace TTT;

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

		Tags.Add( "interactable" );
	}

	public void SetEntity( ModelEntity entity )
	{
		RealEntity = entity;
		Model = entity.Model;
		RenderColor = RenderColor.WithAlpha( 0.5f );
	}

	public void ShowValid()
	{
		RenderColor = Color.Green.WithAlpha( 0.5f );
	}

	public void ShowInvalid()
	{
		RenderColor = Color.Red.WithAlpha( 0.5f );
	}

	public bool IsPlacementValid( ref TraceResult trace )
	{
		var position = trace.EndPosition;
		var bounds = CollisionBounds;
		var entities = Entity.FindInBox( bounds + position );

		return !entities.Any();
	}
}
