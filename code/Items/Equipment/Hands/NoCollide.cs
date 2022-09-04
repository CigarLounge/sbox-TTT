using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class NoCollide : EntityComponent<ModelEntity>
{
	private readonly List<string> _savedTags = new();
	private TimeUntil _timeUntilRemoval = 0.1f;
	private ModelEntity _entity;

	protected override void OnActivate()
	{
		if ( !Host.IsServer )
			return;

		_entity = Entity;

		// Remove all tags to ensure it doesn't collide with anything.
		// Store the tags and restore them OnDeactivate.
		foreach ( var tag in _entity.Tags.List )
		{
			_savedTags.Add( tag );
			_entity.Tags.Remove( tag );
		}

		_entity.Tags.Add( "nocollide" );
		_entity.RenderColor = _entity.RenderColor.WithAlpha( 0.5f );
	}

	protected override void OnDeactivate()
	{
		if ( !Host.IsServer || !_entity.IsValid() )
			return;

		foreach ( var tag in _savedTags )
			_entity.Tags.Add( tag );

		_entity.RenderColor = _entity.RenderColor.WithAlpha( 1f );
		_entity.Tags.Remove( "nocollide" );
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !_entity.IsValid() )
			return;

		// FindInBox includes the entity itself, therefore Count() == 1.
		if ( _timeUntilRemoval && Sandbox.Entity.FindInBox( _entity.CollisionBounds + _entity.Position ).Count() == 1 )
			_entity.Components.RemoveAny<NoCollide>();
	}
}
