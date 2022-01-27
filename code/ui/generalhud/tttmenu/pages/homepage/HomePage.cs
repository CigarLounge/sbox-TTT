using Sandbox;
using Sandbox.UI;

namespace TTT.UI.Menu
{
	[UseTemplate]
	public partial class HomePage : Panel
	{
		private TranslationButton ShopEditorButton { get; set; }

		public void GoToSettingsPage()
		{
			TTTMenu.Instance.AddPage( new SettingsPage() );
		}

		public void GoToKeyBindingsPage()
		{
			TTTMenu.Instance.AddPage( new KeyBindingsPage() );
		}


		public void GoToShopEditor()
		{
			// Call to server which sends down server data and then adds the ShopEditorPage.
			ShopEditorPage.ServerRequestShopEditorAccess();
		}

		public HomePage()
		{
			if ( Local.Client.HasPermission( "shopeditor" ) )
			{
				ShopEditorButton.RemoveClass( "inactive" );
			}
		}

		public void GoToComponentTesting()
		{
			TTTMenu.Instance.AddPage( new ComponentTestingPage() );
		}
	}
}
