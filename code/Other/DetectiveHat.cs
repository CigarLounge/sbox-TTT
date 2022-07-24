using Sandbox;

namespace TTT;

[HideInEditor]
[EditorModel( "models/detective_hat/detective_hat.vmdl" )]
[Title( "Detective Hat" )]
public partial class DetectiveHat : Prop, IEntityHint
{
	public static readonly string Path = "models/detective_hat/detective_hat.vmdl";
	private static readonly Model _worldModel = Model.Load( Path );

	public override void Spawn()
	{
		Tags.Add( "trigger" );
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public void PutOn( Player player )
	{
		player.AttachClothing( Path );
		Delete();
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.Hint( DisplayInfo.For( this ).Name );
}
