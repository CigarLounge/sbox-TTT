using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_poltergeist" )]
[HideInEditor]
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
