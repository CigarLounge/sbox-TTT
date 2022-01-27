using System.Collections.Generic;

using Sandbox;

using TTT.Globalization;
using TTT.Globals;
using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public partial class PlayerCorpse : ModelEntity, IEntityHint
	{
		public TTTPlayer DeadPlayer { get; set; }
		public List<Particles> Ropes = new();
		public List<PhysicsJoint> RopeSprings = new();
		public string KillerWeapon { get; set; }
		public bool IsIdentified { get; set; } = false;
		public bool WasHeadshot { get; set; } = false;
		public bool Suicide { get; set; } = false;
		public float Distance { get; set; } = 0f;
		public float KilledTime { get; private set; }
		public string[] Perks { get; set; }

		public PlayerCorpse()
		{
			MoveType = MoveType.Physics;
			UsePhysicsCollision = true;

			SetInteractsAs( CollisionLayer.Debris );
			SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
			SetInteractsExclude( CollisionLayer.Player );

			KilledTime = Time.Now;
		}

		public void CopyFrom( TTTPlayer player )
		{
			DeadPlayer = player;

			SetModel( player.GetModelName() );
			TakeDecalsFrom( player );

			this.CopyBonesFrom( player );
			this.SetRagdollVelocityFrom( player );

			List<C4Entity> attachedC4s = new();

			foreach ( Entity child in player.Children )
			{
				if ( child is C4Entity c4 && c4.AttachedBone > -1 )
				{
					attachedC4s.Add( c4 );
				}

				if ( child is ModelEntity e )
				{
					string model = e.GetModelName();

					if ( model == null || !model.Contains( "clothes" ) )
					{
						continue;
					}

					ModelEntity clothing = new();
					clothing.SetModel( model );
					clothing.SetParent( this, true );
				}
			}

			foreach ( C4Entity c4 in attachedC4s )
			{
				c4.SetParent( this, c4.AttachedBone );
			}
		}

		public void ApplyForceToBone( Vector3 force, int forceBone )
		{
			PhysicsGroup.AddVelocity( force );

			if ( forceBone < 0 )
			{
				return;
			}

			PhysicsBody corpse = GetBonePhysicsBody( forceBone );

			if ( corpse != null )
			{
				corpse.ApplyForce( force * 1000 );
			}
			else
			{
				PhysicsGroup.AddVelocity( force );
			}
		}

		public void ClearAttachments()
		{
			foreach ( Particles rope in Ropes )
			{
				rope.Destroy( true );
			}

			foreach ( PhysicsJoint spring in RopeSprings )
			{
				spring.Remove();
			}

			Ropes.Clear();
			RopeSprings.Clear();
		}

		protected override void OnDestroy()
		{
			ClearAttachments();
		}

		public void CopyConfirmationData( ConfirmationData confirmationData )
		{
			IsIdentified = confirmationData.Identified;
			WasHeadshot = confirmationData.Headshot;
			KilledTime = confirmationData.Time;
			Distance = confirmationData.Distance;
			Suicide = confirmationData.Suicide;
		}

		public ConfirmationData GetConfirmationData()
		{
			return new ConfirmationData
			{
				Identified = IsIdentified,
				Headshot = WasHeadshot,
				Time = KilledTime,
				Distance = Distance,
				Suicide = Suicide
			};
		}

		public float HintDistance => 80f;

		public TranslationData TextOnTick => new( IsIdentified ? "CORPSE_INSPECT" : "CORPSE_IDENTIFY", new object[] { Input.GetKeyWithBinding( "+iv_use" ).ToUpper() } );

		public bool CanHint( TTTPlayer client ) => true;

		public EntityHintPanel DisplayHint( TTTPlayer client )
		{
			return new Hint( TextOnTick );
		}

		public void Tick( TTTPlayer confirmingPlayer )
		{
			using ( Prediction.Off() )
			{
				if ( IsClient && !Input.Down( InputButton.Use ) )
				{
					if ( InspectMenu.Instance != null )
					{
						InspectMenu.Instance.Enabled = false;
					}

					return;
				}

				if ( IsServer && !IsIdentified && confirmingPlayer.LifeState == LifeState.Alive && Input.Down( InputButton.Use ) )
				{
					IsIdentified = true;

					// TODO: Handle player disconnects.
					if ( DeadPlayer != null && DeadPlayer.IsValid() )
					{
						DeadPlayer.IsConfirmed = true;
						DeadPlayer.CorpseConfirmer = confirmingPlayer;

						int credits = DeadPlayer.Credits;

						if ( credits > 0 )
						{
							confirmingPlayer.Credits += credits;
							DeadPlayer.Credits = 0;
							DeadPlayer.CorpseCredits = credits;
						}

						RPCs.ClientConfirmPlayer( confirmingPlayer, this, DeadPlayer, DeadPlayer.Role.Name, DeadPlayer.Team.Name, GetConfirmationData(), KillerWeapon, Perks );
					}
				}

				if ( Input.Down( InputButton.Use ) && IsIdentified )
				{
					TTTPlayer.ClientEnableInspectMenu( this );
				}
			}
		}
	}
}
