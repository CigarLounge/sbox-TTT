using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_c4", Title = "C4" )]
public partial class C4 : Droppable<C4Entity>
{
	protected override string ModelPath => "models/c4/c4.vmdl";
}
