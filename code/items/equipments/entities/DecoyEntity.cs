using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_equipment_decoy_ent", Title = "Decoy" )]
	[Precached( "models/entities/decoy.vmdl" )]
	[Hammer.Skip]
	public partial class DecoyEntity : Prop
	{
		public override string ModelPath => "models/entities/decoy.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( ModelPath );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}
	}
}
