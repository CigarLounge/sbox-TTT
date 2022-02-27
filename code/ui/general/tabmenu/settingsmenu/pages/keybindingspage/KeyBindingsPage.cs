using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class KeyBindingsPage : Panel
{
	public KeyBindingsPage()
	{
		StyleSheet.Load( "/ui/general/tabmenu/settingsmenu/pages/KeyBindingsPage/KeyBindingsPage.scss" );

		Add.Label( "You can change your bindings in the s&box options menu or through console." );
		Add.Label( "" );

		Add.Label( "Movement", "h1" );
		CreateBinding( this, "Forward", new() { InputButton.Forward } );
		CreateBinding( this, "Back", new() { InputButton.Back } );
		CreateBinding( this, "Left", new() { InputButton.Left } );
		CreateBinding( this, "Right", new() { InputButton.Right } );
		CreateBinding( this, "Jump", new() { InputButton.Jump } );
		CreateBinding( this, "Crouch", new() { InputButton.Duck } );
		CreateBinding( this, "Walk", new() { InputButton.Run } );
		Add.Label( "" );

		Add.Label( "Weapons", "h1" );
		CreateBinding( this, "Shoot", new() { InputButton.Attack1 } );
		CreateBinding( this, "Reload", new() { InputButton.Reload } );
		CreateBinding( this, "Drop Weapon", new() { InputButton.Drop } );
		CreateBinding( this, "Drop Ammo", new() { InputButton.Run, InputButton.Drop } );
		CreateBinding( this, "Weapon Swap", new() { InputButton.Menu } );
		Add.Label( "" );

		Add.Label( "Actions", "h1" );
		CreateBinding( this, "Use", new() { InputButton.Use } );
		CreateBinding( this, "Flashlight", new() { InputButton.Flashlight } );
		CreateBinding( this, "Disguiser", new() { InputButton.Grenade } );
		Add.Label( "" );

		Add.Label( "Communication", "h1" );
		CreateBinding( this, "Voice Chat", new() { InputButton.Voice } );
		CreateBinding( this, "Team Text Chat", new() { InputButton.Score } );
		CreateBinding( this, "Team Voice Chat", new() { InputButton.Walk } );
		Add.Label( "" );

		Add.Label( "Menus", "h1" );
		CreateBinding( this, "Scoreboard & Settings", new() { InputButton.Score } );
		CreateBinding( this, "Quick Shop", new() { InputButton.View } );
		Add.Label( "" );
	}

	private static void CreateBinding( Panel parent, string actionName, List<InputButton> bindings )
	{
		Panel wrapper = new( parent );
		wrapper.AddClass( "wrapper" );

		wrapper.Add.Label( actionName );
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
