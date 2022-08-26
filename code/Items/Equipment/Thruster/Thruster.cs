using Sandbox;

namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_thruster" )]
[Title( "Thruster" )]
public partial class Thruster : Deployable<ThrusterEntity>
{
	[Net, Local]
	public int Ammo { get; protected set; } = 5;
	public override string SlotText => $"{Ammo}";

	protected override bool CanDrop => false;

	protected override ThrusterEntity Deploy( TraceResult trace )
	{
		var dropped = new ThrusterEntity
		{
			Owner = this,
			PhysicsEnabled = true,
			Position = trace.EndPosition,
			Velocity = 0,
			Parent = trace.Entity
		};

		Ammo -= 1;

		if ( Ammo <= 0 )
			Delete();

		return dropped;
	}

	protected override bool IsPlacementValid( TraceResult trace )
	{
		return trace.Body.BodyType != PhysicsBodyType.Static;
	}
}
