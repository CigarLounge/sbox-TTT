using System.Linq;
using Sandbox;

namespace TTT;

/// <summary>
/// If this component is present on a prop any "PhysicsImpact" damage dealt to a player will be ignored.
/// </summary>
public partial class IgnoreDamage : EntityComponent<ModelEntity>
{
	private ModelEntity _entity;

	protected override void OnActivate()
	{
		if ( !Game.IsServer )
			return;

		_entity = Entity;
		_entity.Tags.Add( Strings.Tags.IgnoreDamage );
	}

	protected override void OnDeactivate()
	{
		if ( !Game.IsServer || !_entity.IsValid() )
			return;

		_entity.Tags.Remove( Strings.Tags.IgnoreDamage );
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !_entity.IsValid() )
			return;

		// Once it no longer clips with any other entity remove the component.
		// FindInBox includes the entity itself. therefore the Count() == 1.
		if ( Sandbox.Entity.FindInBox( _entity.CollisionBounds + _entity.Position ).Count() == 1 )
			_entity.Components.RemoveAny<IgnoreDamage>();
	}
}
