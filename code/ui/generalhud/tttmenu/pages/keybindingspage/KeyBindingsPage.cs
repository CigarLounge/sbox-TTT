using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Globalization;

namespace TTT.UI.Menu
{
	public partial class KeyBindingsPage : Panel
	{
		public KeyBindingsPage()
		{
			StyleSheet.Load( "/ui/generalhud/tttmenu/pages/KeyBindingsPage/KeyBindingsPage.scss" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_DESCRIPTION" ) );
			Add.Label( "" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_MOVEMENT" ), "h1" );
			CreateBinding( this, "MENU_KEYBINDINGS_FORWARD", new() { InputButton.Forward } );
			CreateBinding( this, "MENU_KEYBINDINGS_BACK", new() { InputButton.Back } );
			CreateBinding( this, "MENU_KEYBINDINGS_LEFT", new() { InputButton.Left } );
			CreateBinding( this, "MENU_KEYBINDINGS_RIGHT", new() { InputButton.Right } );
			CreateBinding( this, "MENU_KEYBINDINGS_JUMP", new() { InputButton.Jump } );
			CreateBinding( this, "MENU_KEYBINDINGS_CROUCH", new() { InputButton.Duck } );
			CreateBinding( this, "MENU_KEYBINDINGS_SPRINT", new() { InputButton.Run } );
			Add.Label( "" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_WEAPONS" ), "h1" );
			CreateBinding( this, "MENU_KEYBINDINGS_FIRE", new() { InputButton.Attack1 } );
			CreateBinding( this, "MENU_KEYBINDINGS_RELOAD", new() { InputButton.Reload } );
			CreateBinding( this, "MENU_KEYBINDINGS_DROP_WEAPON", new() { InputButton.Drop } );
			CreateBinding( this, "MENU_KEYBINDINGS_DROP_AMMO", new() { InputButton.Run, InputButton.Drop } );
			Add.Label( "" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_ACTIONS" ), "h1" );
			CreateBinding( this, "MENU_KEYBINDINGS_USE", new() { InputButton.Use } );
			CreateBinding( this, "MENU_KEYBINDINGS_FLASHLIGHT", new() { InputButton.Flashlight } );
			CreateBinding( this, "MENU_KEYBINDINGS_DISGUISER", new() { InputButton.Grenade } );
			Add.Label( "" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_COMMUNICATION" ), "h1" );
			CreateBinding( this, "MENU_KEYBINDINGS_VOICE_CHAT", new() { InputButton.Voice } );
			CreateBinding( this, "MENU_KEYBINDINGS_TEAM_VOICE_CHAT", new() { InputButton.Walk } );
			CreateBinding( this, "MENU_KEYBINDINGS_TEAM_TEXT_CHAT", new() { InputButton.Score } );
			Add.Label( "" );

			Add.TranslationLabel( new TranslationData( "MENU_KEYBINDINGS_MENUS" ), "h1" );
			CreateBinding( this, "MENU_KEYBINDINGS_SCOREBOARD", new() { InputButton.Score } );
			CreateBinding( this, "MENU_KEYBINDINGS_MENU", new() { InputButton.Menu } );
			CreateBinding( this, "MENU_KEYBINDINGS_QUICK_SHOP", new() { InputButton.View } );
			Add.Label( "" );
		}

		private static void CreateBinding( Panel parent, string actionName, List<InputButton> bindings )
		{
			Panel wrapper = new( parent );
			wrapper.AddClass( "wrapper" );

			wrapper.Add.TranslationLabel( new TranslationData( actionName ) );
			wrapper.Add.Label( ": " );

			for ( int i = 0; i < bindings.Count; ++i )
			{
				var image = wrapper.Add.Image();
				image.Texture = Input.GetGlyph( bindings[i] );
				wrapper.Add.Label( $" (+iv_{bindings[i].ToString().ToLower()}) " );

				// Don't show a + if it's the last binding in the list.
				if ( i != bindings.Count - 1 )
				{
					wrapper.Add.Label( " + " );
				}
			}
		}
	}
}
