using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Scope : Panel
{
	private readonly Carriable _carriable;
	private readonly Image _lens;

	public Scope( Carriable carriable, string scopeTexture )
	{
		StyleSheet.Load( "/ui/player/scope/Scope.scss" );
		_carriable = carriable;

		Add.Panel( "leftBar" );
		_lens = Add.Image( scopeTexture, "lens" );
		Add.Panel( "rightBar" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player || player.ActiveChild != _carriable )
			return;

		Style.Opacity = Input.Down( InputButton.Attack2 ) ? 1 : 0;
		if ( Style.Opacity != 1 )
			return;

		var scopeSize = Screen.Height * ScaleFromScreen;
		_lens.Style.Width = Length.Pixels( scopeSize );
		_lens.Style.Height = Length.Pixels( scopeSize );
	}
}
