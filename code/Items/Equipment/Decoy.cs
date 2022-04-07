using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_decoy", Title = "Decoy" )]
public partial class Decoy : Droppable<DecoyEntity>
{
	protected override string ModelPath => "models/decoy/decoy.vmdl";
}
