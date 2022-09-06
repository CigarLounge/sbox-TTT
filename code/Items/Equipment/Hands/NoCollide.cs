using Sandbox;

namespace TTT;

public partial class NoCollide : EntityComponent<ModelEntity>
{
	private ModelEntity _entity;

	protected override void OnActivate()
	{
		if ( !Host.IsServer )
			return;

		_entity = Entity;
		_entity.Tags.Add( "nocollide" );
	}

	protected override void OnDeactivate()
	{
		if ( !Host.IsServer || !_entity.IsValid() )
			return;

		_entity.Tags.Remove( "nocollide" );
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !_entity.IsValid() )
			return;

		if ( _entity.Velocity.Length == 0 )
			_entity.Components.RemoveAny<NoCollide>();
	}
}
