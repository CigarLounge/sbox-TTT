using Sandbox;

namespace TTT;

[GameResource( "Weapon", "weapon", "TTT weapon template.", Icon = "🔫" )]
public class WeaponInfo : CarriableInfo
{
	[Category( "Sounds" ), ResourceType( "sound" )]
	public string FireSound { get; set; } = "";

	[Category( "Sounds" ), ResourceType( "sound" )]
	public string DryFireSound { get; set; } = "";

	[Category( "Important" )]
	[Description( "The ammo type. Set this to None if you want the ammo for your weapon to be limited (you can't pick up or drop ammo for it)." )]
	public AmmoType AmmoType { get; set; } = AmmoType.None;

	[Category( "Important" )]
	public FireMode FireMode { get; set; } = FireMode.Automatic;

	[Category( "Stats" )]
	[Description( "The amount of bullets that come out in one shot." )]
	public int BulletsPerFire { get; set; } = 1;

	[Category( "Stats" )]
	public int ClipSize { get; set; } = 30;

	[Category( "Stats" )]
	public float Damage { get; set; } = 20;

	[Category( "Stats" )]
	public float DamageFallOffStart { get; set; } = 0f;

	[Category( "Stats" )]
	public float DamageFallOffEnd { get; set; } = 0f;

	[Category( "Stats" )]
	public float HeadshotMultiplier { get; set; } = 1f;

	[Category( "Stats" )]
	public float Spread { get; set; } = 0f;

	[Category( "Stats" )]
	public float PrimaryRate { get; set; } = 0f;

	[Category( "Stats" )]
	public float SecondaryRate { get; set; } = 0f;

	[Category( "Stats" )]
	[Description( "The amount of ammo this weapon spawns with if the ammo type is set to None." )]
	public int ReserveAmmo { get; set; } = 0;

	[Category( "Stats" )]
	public float ReloadTime { get; set; } = 2f;

	[Category( "Stats" )]
	public float VerticalRecoil { get; set; } = 0f;

	[Category( "Stats" )]
	public float HorizontalRecoilRange { get; set; } = 0f;

	[Category( "Stats" )]
	public float RecoilRecoveryScale { get; set; } = 0f;

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string EjectParticle { get; set; } = "";

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string MuzzleFlashParticle { get; set; } = "";

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string TracerParticle { get; set; } = "";

	protected override void PostLoad()
	{
		base.PostLoad();

		Precache.Add( EjectParticle );
		Precache.Add( MuzzleFlashParticle );
		Precache.Add( TracerParticle );
	}
}
