using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_bekas.vmdl" )]
[Library( "ttt_weapon_bekas", Title = "Bekas-M" )]
public partial class Bekas : Weapon
{
	private bool _attackedDuringReload = false;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		_attackedDuringReload = false;
		TimeSinceReload = 0f;
	}

	protected override bool CanReload()
	{
		if ( !base.CanReload() )
			return false;

		var rate = Info.PrimaryRate;
		if ( rate <= 0 )
			return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( IsReloading && Input.Pressed( InputButton.Attack1 ) )
			_attackedDuringReload = true;
	}

	protected override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;

		AmmoClip += TakeAmmo( 1 );

		if ( !_attackedDuringReload && AmmoClip < Info.ClipSize && Owner.AmmoCount( Info.AmmoType ) > 0 )
			Reload();
		else
			FinishReload();

		_attackedDuringReload = false;
	}

	[ClientRpc]
	public void FinishReload()
	{
		ViewModelEntity?.SetAnimParameter( "reload_finished", true );
	}
}
