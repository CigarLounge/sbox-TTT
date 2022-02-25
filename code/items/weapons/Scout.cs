using Sandbox;
using Sandbox.UI;
using TTT.UI;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_spr.wmdl" )]
[Library( "ttt_weapon_scout", Title = "Scout" )]
public partial class Scout : Weapon
{
	private float _defaultFOV;
	private bool _isScoped;
	private Scope _sniperScopePanel;

	public override void Simulate( Client owner )
	{
		if ( owner.Pawn is Player player )
		{
			if ( Input.Pressed( InputButton.Attack2 ) )
				OnScopeStart( player );

			if ( Input.Released( InputButton.Attack2 ) )
				OnScopeEnd( player );
		}

		base.Simulate( owner );
	}

	public override void CreateHudElements()
	{
		if ( Hud.GeneralHud.Instance == null )
			return;

		_sniperScopePanel = new Scope( "/ui/scout_scope.png" )
		{
			Parent = Hud.GeneralHud.Instance
		};
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		if ( Owner is Player player )
			_defaultFOV = player.Camera.FieldOfView;
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );
		_sniperScopePanel?.Delete();
	}

	private void OnScopeStart( Player player )
	{
		player.Camera.FieldOfView = player.Camera.FieldOfView = 10f;

		if ( IsServer )
			return;

		ViewModelEntity.RenderColor = Color.Transparent;
		HandsModelEntity.RenderColor = Color.Transparent;
		_isScoped = true;
		_sniperScopePanel.Show();
	}

	private void OnScopeEnd( Player player )
	{
		player.Camera.FieldOfView = player.Camera.FieldOfView = _defaultFOV;

		if ( IsServer )
			return;

		ViewModelEntity.RenderColor = Color.White;
		HandsModelEntity.RenderColor = Color.White;
		_isScoped = false;
		_sniperScopePanel.Hide();
	}

	[Event.BuildInput]
	protected new virtual void BuildInput( InputBuilder builder )
	{
		if ( _isScoped )
		{
			builder.AnalogLook *= 0.2f;
		}
	}
}
