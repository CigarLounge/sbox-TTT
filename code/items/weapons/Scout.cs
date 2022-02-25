using Sandbox;
using Sandbox.UI;
using TTT.UI;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_spr.wmdl" )]
[Library( "ttt_weapon_scout", Title = "Scout" )]
public partial class Scout : Weapon
{
	private Panel _sniperScopePanel;

	public override void CreateHudElements()
	{
		if ( Hud.GeneralHud.Instance == null )
			return;

		_sniperScopePanel = new Scope( this, "/ui/scout_scope.png" )
		{
			Parent = Hud.GeneralHud.Instance
		};
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );
		_sniperScopePanel?.Delete();
	}

	[Event.BuildInput]
	protected new virtual void BuildInput( InputBuilder builder )
	{
		if ( true )
		{
			builder.AnalogLook *= 0.2f;
		}
	}
}
