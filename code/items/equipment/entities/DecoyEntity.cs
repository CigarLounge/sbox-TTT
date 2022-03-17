using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/decoy/decoy.vmdl" )]
[Library( "ttt_entity_decoy", Title = "Decoy" )]
public partial class DecoyEntity : Prop, IEntityHint
{
	private const string WorldModel = "models/decoy/decoy.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	string IEntityHint.TextOnTick => "Decoy";

	bool IEntityHint.CanHint( Player player )
	{
		return true;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Hint( (this as IEntityHint).TextOnTick );
	}
}
