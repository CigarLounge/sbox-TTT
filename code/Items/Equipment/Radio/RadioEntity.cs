using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_radio" )]
[EditorModel( "models/radio/radio.vmdl" )]
[Title( "Radio" )]
public partial class RadioEntity : Prop, IEntityHint, IUse
{
	private static readonly Model _worldModel = Model.Load( "models/radio/radio.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 100f;
	}

	protected override void OnDestroy()
	{
		Owner?.Components.RemoveAny<RadioComponent>();

		base.OnDestroy();
	}

	bool IUse.OnUse( Entity user )
	{
		var player = user as Player;
		player.Inventory.Add( new Radio() );
		Delete();

		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user is Player && (Owner is null || user == Owner);
	}

	[ConCmd.Server]
	public static void PlayRadio( int id, string sound )
	{
		if ( Entity.FindByIndex( id ) is not RadioEntity radio || radio.Owner != ConsoleSystem.Caller.Pawn )
			return;

		radio.PlaySound( sound );
	}
}
