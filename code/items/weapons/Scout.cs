using System;

using Sandbox;

using SWB_Base;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "ttt_weapon_scout", Title = "Scout" )]
	[Shop( SlotType.Primary, 100 )]
	[Spawnable]
	[Precached( "models/weapons/v_spr.vmdl", "models/weapons/w_spr.vmdl",
	"particles/swb/muzzle/flash_large.vpcf", "particles/swb/tracer/tracer_large.vpcf" )]
	[Hammer.EditorModel( "models/weapons/w_spr.vmdl" )]
	public class Scout : WeaponBaseSniper, ICarriableItem, IEntityHint
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( Scout ) );
		public Type DroppedType => typeof( SniperAmmo );

		public override float TuckRange => -1f;
		public override int Bucket => 5;
		public override HoldType HoldType => HoldType.Rifle;
		public override string HandsModelPath => "models/weapons/v_arms_ter.vmdl";
		public override string ViewModelPath => "models/weapons/v_spr.vmdl";
		public override AngPos ViewModelOffset => new()
		{
			Angle = new Angles( 0, -5, 0 ),
			Pos = new Vector3( -5, 0, 0 )
		};
		public override string WorldModelPath => "models/weapons/w_spr.vmdl";
		public override string Icon => "";
		public override int FOV => 75;
		public override int ZoomFOV => 75;
		public override float WalkAnimationSpeedMod => 0.8f;
		public override float AimSensitivity => 0.25f;

		public override string LensTexture => "/materials/swb/scopes/swb_lens_hunter.png";
		public override string ScopeTexture => "/materials/swb/scopes/swb_scope_hunter.png";
		public override string ZoomInSound => "swb_sniper.zoom_in";
		public override float ZoomAmount => 15f;
		public override bool UseRenderTarget => false;

		public Scout()
		{
			General = new WeaponInfo
			{
				DrawTime = 1.5f,
				ReloadTime = 3f,

				BoltBackTime = 1f,
				BoltBackEjectDelay = 0.2f
			};

			Primary = new ClipInfo
			{
				Ammo = 5,
				AmmoType = AmmoType.Sniper,
				ClipSize = 5,

				BulletSize = 5f,
				Damage = 100f,
				Force = 7f,
				Spread = 0.75f,
				Recoil = 2f,
				RPM = 125,
				FiringType = FiringType.semi,
				ScreenShake = new ScreenShake
				{
					Length = 0.5f,
					Speed = 4.0f,
					Size = 1f,
					Rotation = 0.5f
				},

				DryFireSound = "dryfire_rifle-1",
				ShootSound = "spr_fire-1",

				BulletEjectParticle = "particles/pistol_ejectbrass.vpcf",
				MuzzleFlashParticle = "particles/swb/muzzle/flash_large.vpcf",
				BulletTracerParticle = "particles/swb/tracer/tracer_large.vpcf",

				InfiniteAmmo = 0
			};
		}

		public override void Simulate( Client client )
		{
			WeaponGenerics.Simulate( client, Primary, DroppedType );
			base.Simulate( client );
		}

		public float HintDistance => TTTPlayer.INTERACT_DISTANCE;
		public string TextOnTick => WeaponGenerics.PickupText( _data.Library.Title );
		bool ICarriableItem.CanDrop() { return true; }
		public bool CanHint( TTTPlayer player ) { return true; }
		public EntityHintPanel DisplayHint( TTTPlayer player ) { return new Hint( TextOnTick ); }
		public void Tick( TTTPlayer player ) { WeaponGenerics.Tick( player, this ); }
	}
}
