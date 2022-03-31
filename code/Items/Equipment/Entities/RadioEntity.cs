using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/radio/radio.vmdl" )]
[Library( "ttt_entity_radio", Title = "Radio" )]
public partial class RadioEntity : DroppableEntity, IEntityHint, IUse
{
	private static readonly Model WorldModel = Model.Load( "models/radio/radio.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
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
		return user is Player && (EquipmentOwner is null || user == EquipmentOwner);
	}
}
