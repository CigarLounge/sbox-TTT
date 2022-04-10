using Sandbox;

namespace TTT.Entities;

[Hammer.EditorModel( "models/decoy/decoy.vmdl" )]
[Library( "ttt_entity_decoy", Title = "Decoy" )]
public class Decoy : Prop, IEntityHint, IUse
{
	private static readonly Model WorldModel = Model.Load( "models/decoy/decoy.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 100f;
	}

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		player.Inventory.Add( new Items.Decoy() );
		Delete();

		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user is Player && (Owner is null || user == Owner);
	}
}
