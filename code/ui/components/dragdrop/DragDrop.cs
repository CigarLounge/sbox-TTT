using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class DragDrop : Panel
    {
        public string DragDropGroupName { get; set; } = "";

        public DragDrop(Panel parent = null) : base(parent)
        {
            AddClass("dragdrop");
        }
    }
}
