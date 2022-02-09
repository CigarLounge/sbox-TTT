using Sandbox.UI;

namespace TTT.UI
{
	public partial class DragDrop : Panel
	{
		public string DragDropGroupName { get; set; } = "";

		public DragDrop( Panel parent = null ) : base( parent )
		{
			AddClass( "dragdrop" );
		}
	}
}
