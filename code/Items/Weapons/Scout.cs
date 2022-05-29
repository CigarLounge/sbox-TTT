using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_weapon_scout" )]
[EditorModel( "models/weapons/w_spr.vmdl" )]
[HammerEntity]
[Title( "Scout" )]
public class Scout : Weapon
{
	public bool IsScoped { get; private set; }

	private float _defaultFOV;
	private UI.Scope _sniperScopePanel;

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		IsScoped = false;
		_defaultFOV = player.Camera.FieldOfView;
	}

	public override void Simulate( Client client )
	{
		if ( IsClient && Input.Pressed( InputButton.SecondaryAttack ) )
		{
			if ( Prediction.FirstTime )
			{
				SetScoped( !IsScoped );
				PlaySound( Strings.ScopeInSound );
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

		PreviousOwner.Camera.FieldOfView = _defaultFOV;
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

		Owner.Camera.FieldOfView = isScoped ? 10f : _defaultFOV;
	}
}
