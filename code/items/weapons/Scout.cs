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

		if ( ent is Player player )
			_defaultFOV = player.CameraMode.FieldOfView;
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		if ( ent is Player player )
			OnScopeEnd( player );

		_sniperScopePanel?.Delete();

		base.ActiveEnd( ent, dropped );
	}

	private void OnScopeStart( Player player )
	{
		player.CameraMode.FieldOfView = player.CameraMode.FieldOfView = 10f;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = false;
		HandsModelEntity.EnableDrawing = false;
		_isScoped = true;
		_sniperScopePanel.Show();
	}

	private void OnScopeEnd( Player player )
	{
		player.CameraMode.FieldOfView = player.CameraMode.FieldOfView = _defaultFOV;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = true;
		HandsModelEntity.EnableDrawing = true;
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
