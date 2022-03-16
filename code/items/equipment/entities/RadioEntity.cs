using Sandbox;
using TTT.UI;

namespace TTT;

[Hammer.EditorModel( "models/radio/radio.vmdl" )]
[Library( "ttt_entity_radio", Title = "Radio" )]
public partial class RadioEntity : Prop, IEntityHint
{
	public Player RadioOwner { get; set; }
	private const string WorldModel = "models/radio/radio.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	protected override void OnDestroy()
	{
		RadioOwner?.Components.RemoveAny<RadioComponent>();
		base.OnDestroy();
	}

	string IEntityHint.TextOnTick => "Radio";

	public bool CanHint( Player player )
	{
		return true;
	}

	public EntityHintPanel DisplayHint( Player player )
	{
		return new Hint( (this as IEntityHint).TextOnTick );
	}
}
