using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SettingsMenu : Panel
{
	public static SettingsMenu Instance;

	public Panel ActivePage { get; private set; }

	/// <summary>
	/// "Children" is used as a "stack" where the last element in the list
	/// is the page that is currently showing.
	/// </summary>
	private Panel Pages { get; set; }

	private Panel BackgroundPanel { get; set; }

	private bool HasPreviousPages { get => Pages.ChildrenCount > 1; }

	private Button BackButton { get; set; }
	private Button HomeButton { get; set; }

	public SettingsMenu()
	{
		Instance = this;

		AddPage( new HomePage() );

		BackgroundPanel.AddClass( "disabled" );
	}

	/// <summary>
	/// Add and show a new page to the menu.
	/// <param name="page">The panel page to add and show.</param>
	/// </summary>
	public void AddPage( Panel page )
	{
		for ( int i = 0; i < Pages.ChildrenCount; ++i )
		{
			Pages.GetChild( i ).AddClass( "disabled" );
		}

		Pages.AddChild( page );

		BackButton.SetClass( "inactive", !HasPreviousPages );
		HomeButton.SetClass( "inactive", !HasPreviousPages );

		ActivePage = page;
	}

	/// <summary>
	/// Deletes the current page and displays the next page in the stack.
	/// </summary>
	public void PopPage()
	{
		if ( !HasPreviousPages )
		{
			return;
		}

		Pages.GetChild( Pages.ChildrenCount - 1 ).Delete( true );
		Pages.GetChild( Pages.ChildrenCount - 1 ).RemoveClass( "disabled" );

		BackButton.SetClass( "inactive", !HasPreviousPages );
		HomeButton.SetClass( "inactive", !HasPreviousPages );

		ActivePage = Pages.GetChild( Pages.ChildrenCount - 1 ) as Panel;
	}

	/// <summary>
	/// Deletes all pages and goes to the first page in the stack.
	/// </summary>
	public void PopToHomePage()
	{
		while ( HasPreviousPages )
		{
			PopPage();
		}
	}

	[Event.BuildInput]
	private void MenuInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Menu ) )
		{
			BackgroundPanel.SetClass( "disabled", !BackgroundPanel.HasClass( "disabled" ) );
		}
	}
}
