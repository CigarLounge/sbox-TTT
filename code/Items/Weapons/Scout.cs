using Sandbox;

namespace TTT;

[Library( "ttt_weapon_scout", Title = "Scout" )]
public class Scout : Weapon
{
	public bool IsScoped { get; private set; }

	private float _defaultFOV;
	private UI.Scope _sniperScopePanel;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		IsScoped = false;
		_defaultFOV = Owner.CameraMode.FieldOfView;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( IsClient && Input.Pressed( InputButton.Attack2 ) )
		{
			if ( Prediction.FirstTime )
			{
				SetScoped( !IsScoped );
				PlaySound( RawStrings.ScopeInSound );
			}
		}

		base.Simulate( client );
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( IsScoped )
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
		_sniperScopePanel.Delete( true );
	}

	public void SetScoped( bool isScoped )
	{
		IsScoped = isScoped;

		if ( IsScoped )
			_sniperScopePanel.Show();
		else
			_sniperScopePanel.Hide();

		ViewModelEntity.EnableDrawing = !IsScoped;
		HandsModelEntity.EnableDrawing = !IsScoped;

		Owner.CameraMode.FieldOfView = isScoped ? 10f : _defaultFOV;
	}
}
