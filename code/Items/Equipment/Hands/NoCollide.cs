using System.Linq;
using Sandbox;

namespace TTT;

public partial class NoCollide : EntityComponent<ModelEntity>
{
	private TimeUntil _timeUntilRemoval = 0.1f;
	private ModelEntity _entity;

	protected override void OnActivate()
	{
		if ( !Host.IsServer )
			return;

		_entity = Entity;

		foreach ( var tag in _entity.Tags.List )
			_entity.Tags.Set( tag, false );

		_entity.Tags.Add( "nocollide" );
		_entity.RenderColor = _entity.RenderColor.WithAlpha( 0.5f );
	}

	protected override void OnDeactivate()
	{
		if ( !Host.IsServer || !_entity.IsValid() )
			return;

		foreach ( var tag in _entity.Tags.List )
			_entity.Tags.Set( tag, true );

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
