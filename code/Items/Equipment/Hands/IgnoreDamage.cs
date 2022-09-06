using Sandbox;

namespace TTT;

/// <summary>
/// If this component is present on a prop any "PhysicsImpact" damage dealt to a player will be ignored.
/// </summary>
public partial class IgnoreDamage : EntityComponent<ModelEntity>
{
	public const string Tag = "ignoredamage";
	private ModelEntity _entity;

	protected override void OnActivate()
	{
		if ( !Host.IsServer )
			return;

		_entity = Entity;
		_entity.Tags.Add( Tag );
	}

	protected override void OnDeactivate()
	{
		if ( !Host.IsServer || !_entity.IsValid() )
			return;

		_entity.Tags.Remove( Tag );
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !_entity.IsValid() )
			return;

		if ( _entity.Velocity.Length == 0 )
			_entity.Components.RemoveAny<IgnoreDamage>();
	}
}
