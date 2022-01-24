using Sandbox;
using Sandbox.UI;

using TTT.Settings;

namespace TTT.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }
        private Panel Buttons { get; set; }
        private readonly FileSelection _currentFileSelection;

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            SettingsPage.CreateSettings(TabContainer, serverSettings);
            SettingsPage.CreateFileSelectionButtons(Buttons, _currentFileSelection, true);
        }
    }
}
