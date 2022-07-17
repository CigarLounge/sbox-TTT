using Sandbox;
using Sandbox.UI;

namespace TTT;

public static class PanelExtensions
{
	public static void Enabled( this Panel panel, bool enabled )
	{
		panel.SetClass( "disabled", !enabled );
	}

	public static void EnableFade( this Panel panel, bool enabled )
	{
		panel.SetClass( "fade-in", enabled );
		panel.SetClass( "fade-out", !enabled );
	}

	public static bool IsEnabled( this Panel panel )
	{
		if ( panel.HasClass( "disabled" ) )
			return false;

		// s&box bug oddly doesn't detect our disabled panels...
		return panel.IsVisible;
	}

	public static void SetTexture( this Image image, Texture texture )
	{
		image.Style.BackgroundImage = texture ?? Texture.Load( FileSystem.Mounted, "/ui/none.png" );
	}

	public static void SetImage( this Image image, string imagePath )
	{
		image.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, "/ui/none.png" );
	}
}
