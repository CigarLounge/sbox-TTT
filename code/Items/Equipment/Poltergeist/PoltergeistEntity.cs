using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_entity_poltergeist" )]
public partial class PoltergeistEntity : Prop
{
	private static readonly Model WorldModel = Model.Load( "models/poltergeist/poltergeist_attachment.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}
}
