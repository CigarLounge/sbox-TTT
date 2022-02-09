using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public struct ClientData
	{
		public long PlayerId { get; set; }
		public string Name { get; set; }
	}

	public partial class PlayerCorpse : ModelEntity, IEntityHint
	{
		public ClientData DeadPlayerClientData { get; set; }
		public TTTPlayer DeadPlayer { get; set; }
		public List<Particles> Ropes = new();
		public List<PhysicsJoint> RopeSprings = new();
		public string KillerWeapon { get; set; }
		public bool IsIdentified { get; set; } = false;
		public bool WasHeadshot { get; set; } = false;
		public DamageFlags DamageFlag { get; set; } = DamageFlags.Generic;
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
			ClientData clientData = new()
			{
				Name = player.Client.Name,
				PlayerId = player.Client.PlayerId
			};

			DeadPlayerClientData = clientData;
			DeadPlayer = player;


			SetModel( player.GetModelName() );
			TakeDecalsFrom( player );

			this.CopyBonesFrom( player );
			this.SetRagdollVelocityFrom( player );

			List<Entity> attachedEnts = new();

			foreach ( Entity child in player.Children )
			{
				if ( child is ThrownKnife k )
				{
					attachedEnts.Add( k );
					continue;
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

			foreach ( Entity entity in attachedEnts )
			{
				entity.SetParent( this, false );
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
			DamageFlag = confirmationData.DamageFlag;
		}

		public ConfirmationData GetConfirmationData()
		{
			return new ConfirmationData
			{
				Identified = IsIdentified,
				Headshot = WasHeadshot,
				Time = KilledTime,
				Distance = Distance,
				DamageFlag = DamageFlag
			};
		}

		public float HintDistance => TTTPlayer.INTERACT_DISTANCE;

		public string TextOnTick => IsIdentified ? $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to inspect the corpse"
												 : $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to identify the corpse";

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
					FullScreenHintMenu.Instance?.Close();
					return;
				}

				if ( IsServer && !IsIdentified && confirmingPlayer.LifeState == LifeState.Alive && Input.Down( InputButton.Use ) )
				{
					IsIdentified = true;

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

						RPCs.ClientConfirmPlayer( confirmingPlayer, this, DeadPlayer, DeadPlayerClientData.Name, DeadPlayerClientData.PlayerId, DeadPlayer.Role.Name, DeadPlayer.Team.Name, GetConfirmationData(), KillerWeapon, Perks );
					}
				}

				if ( Input.Down( InputButton.Use ) && IsIdentified )
				{
					if ( IsClient )
					{
						FullScreenHintMenu.Instance?.Open( new InspectMenu( this ) );
					}
				}
			}
		}
	}
}
