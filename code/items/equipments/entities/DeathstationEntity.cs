using Sandbox;

using TTT.Globalization;
using TTT.Player;
using TTT.UI;

namespace TTT.Items
{
	[Library( "entity_healthstation" )]
	[Precached( "models/entities/healthstation.vmdl" )]
	public partial class DeathstationEntity : Prop, IEntityHint
	{
		[Net]
		public float StoredDamage { get; set; } = 200f;

		public override string ModelPath => "models/entities/healthstation.vmdl";

		public override void Spawn()
		{
			base.Spawn();

			SetModel( ModelPath );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		}

		public float HintDistance => 80f;

		public TranslationData TextOnTick => new( "HEALTH_STATION", new object[] { Input.GetKeyWithBinding( "+iv_use" ).ToUpper(), $"{StoredDamage}" } );

		public bool CanHint( TTTPlayer client )
		{
			return true;
		}

		public EntityHintPanel DisplayHint( TTTPlayer client )
		{
			return new Hint( TextOnTick );
		}

		public void Tick( TTTPlayer player )
		{
			if ( IsClient )
			{
				return;
			}

			if ( player.LifeState != LifeState.Alive )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Down( InputButton.Use ) )
				{
					if ( StoredDamage > 0 )
					{
						StoredDamage -= player.Health;
						player.TakeDamage( DamageInfo.Generic( 1000 ) );
					}
				}
			}
		}
	}
}
