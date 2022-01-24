using TTT.VisualProgramming;

namespace TTT.UI.VisualProgramming
{
    [Node("main")]
    public class MainNode : Node
    {
        public MainNode() : base(new AllPlayersStackNode())
        {
            SetTitle("MainNode");

            AddSetting<NodeAllPlayersSetting>().ToggleInput(false);
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {

        }
    }
}
