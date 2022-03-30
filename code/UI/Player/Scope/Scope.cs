using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

/// <summary>
/// Pass in a scope texture that will fill the entire player's screen when `Show()` is called.
/// </summary>
public class Scope : Panel
{
	private readonly Image _lens;

	public Scope( string scopeTexture )
	{
		StyleSheet.Load( "/UI/Player/Scope/Scope.scss" );

		Add.Panel( "leftBar" );
		_lens = Add.Image( scopeTexture, "lens" );
		Add.Panel( "rightBar" );

		Style.Opacity = 0f;
	}

	public void Show()
	{
		Style.Opacity = 1f;
		var scopeSize = Screen.Height * ScaleFromScreen;
		_lens.Style.Width = Length.Pixels( scopeSize );
		_lens.Style.Height = Length.Pixels( scopeSize );
		PlaySound( RawStrings.ScopeInSound );
	}

	public void Hide()
	{
		Style.Opacity = 0f;
	}
}
