using Sandbox.UI;

namespace TTT.UI
{
    public class Popup : Sandbox.UI.Popup
    {
        public Popup(Panel sourcePanel, PositionMode position, float offset) : base(sourcePanel, position, offset) { }

        public override void Tick()
        {
            base.Tick();

            // Close any popups if the source panel is closed.
            if (!PopupSource?.IsVisible ?? true)
            {
                CloseAll();
            }
        }
    }
}
