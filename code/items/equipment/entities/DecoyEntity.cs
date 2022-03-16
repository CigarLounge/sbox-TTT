using Sandbox;
using TTT.UI;

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
		Health = 100f;
	}

	string IEntityHint.TextOnTick => "Decoy";

	public bool CanHint( Player player )
	{
		return true;
	}

	public EntityHintPanel DisplayHint( Player player )
	{
		return new Hint( (this as IEntityHint).TextOnTick );
	}
}
