using Sandbox.UI.Construct;

namespace TTT.UI.VisualProgramming
{
    [NodeSetting("all_players")]
    public class NodeAllPlayersSetting : NodeSetting
    {
        public NodeAllPlayersSetting() : base()
        {
            Content.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("All Players");
            });
        }
    }
}
