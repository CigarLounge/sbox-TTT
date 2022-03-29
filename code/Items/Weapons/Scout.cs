using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_spr.vmdl" )]
[Library( "ttt_weapon_scout", Title = "Scout" )]
public partial class Scout : Weapon
{
	private float _defaultFOV;
	private bool _isScoped;
	private UI.Scope _sniperScopePanel;

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		_defaultFOV = Owner.CameraMode.FieldOfView;
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		_isScoped = false;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed >= Info.DeployTime && Input.Pressed( InputButton.Attack2 ) )
		{
			if ( _isScoped )
				OnScopeEnd( Owner );
			else
				OnScopeStart( Owner );
		}

		base.Simulate( client );
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( _isScoped )
			input.ViewAngles = Angles.Lerp( input.OriginalViewAngles, input.ViewAngles, 0.1f );

		base.BuildInput( input );
	}

	public override void CreateHudElements()
	{
		base.CreateHudElements();

		_sniperScopePanel = new UI.Scope( "/ui/scout-scope.png" )
		{
			Parent = Local.Hud
		};
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		(Local.Pawn as Player).CameraMode.FieldOfView = _defaultFOV;
		_sniperScopePanel?.Delete( true );
	}

	private void OnScopeStart( Player player )
	{
		player.CameraMode.FieldOfView = 10f;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = false;
		HandsModelEntity.EnableDrawing = false;
		_isScoped = true;
		_sniperScopePanel.Show();
	}

	private void OnScopeEnd( Player player )
	{
		player.CameraMode.FieldOfView = _defaultFOV;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = true;
		HandsModelEntity.EnableDrawing = true;
		_isScoped = false;
		_sniperScopePanel.Hide();
	}
}
