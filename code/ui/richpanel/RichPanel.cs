using Sandbox.UI;

namespace TTT.UI
{
    public partial class RichPanel : Panel
    {
        public RichPanel(Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/richpanel/RichPanel.scss");
        }
    }
}
