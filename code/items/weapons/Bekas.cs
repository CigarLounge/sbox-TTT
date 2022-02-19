using Sandbox;

namespace TTT;

[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
[Library( "ttt_weapon_bekas", Title = "Bekas-M", Spawnable = true )]
public partial class Bekas : Weapon
{
	private bool _attackedDuringReload = false;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		_attackedDuringReload = false;
		TimeSinceReload = 0f;
	}

	public override bool CanReload()
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

	public override void OnReloadFinish()
	{
		IsReloading = false;

		TimeSincePrimaryAttack = 0;

		AmmoClip += TakeAmmo( 1 );

		if ( !_attackedDuringReload && AmmoClip < Info.ClipSize && (UnlimitedAmmo || ReserveAmmo != 0) )
			Reload();
		else
			FinishReload();

		_attackedDuringReload = false;
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 3 );
	}

	[ClientRpc]
	public void FinishReload()
	{
		ViewModelEntity?.SetAnimBool( "reload_finished", true );
	}
}
