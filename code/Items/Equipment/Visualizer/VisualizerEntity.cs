using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_visualizer" )]
[EditorModel( "models/visualizer/visualizer.vmdl" )]
[Title( "Visualizer" )]
public partial class VisualizerEntity : Prop, IEntityHint, IUse
{
	private static readonly Model _worldModel = Model.Load( "models/visualizer/visualizer.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 100f;
	}

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		player.Inventory.Add( new Visualizer() );
		Delete();

		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user is Player && (Owner is null || user == Owner);
	}
}
