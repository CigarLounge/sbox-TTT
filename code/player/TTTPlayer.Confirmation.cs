using Sandbox;

using TTT.Globals;
using TTT.Items;
using TTT.UI;

namespace TTT.Player
{
	public struct ConfirmationData
	{
		public bool Identified;
		public bool Headshot;
		public DamageFlags DamageFlag;
		public float Time;
		public float Distance;
	}

	public partial class TTTPlayer
	{
		public PlayerCorpse PlayerCorpse { get; set; }

		[Net]
		public int CorpseCredits { get; set; } = 0;

		public bool IsConfirmed = false;

		public bool IsMissingInAction = false;

		public TTTPlayer CorpseConfirmer = null;

		public void RemovePlayerCorpse()
		{
			if ( PlayerCorpse == null || !PlayerCorpse.IsValid() )
			{
				return;
			}

			PlayerCorpse.Delete();
			PlayerCorpse = null;
		}

		private void BecomePlayerCorpseOnServer( Vector3 force, int forceBone )
		{
			PlayerCorpse corpse = new()
			{
				Position = Position,
				Rotation = Rotation
			};

			corpse.KillerWeapon = LastDamageWeapon?.Data.Title;
			corpse.WasHeadshot = LastDamageWasHeadshot;
			corpse.Distance = LastDistanceToAttacker;
			corpse.DamageFlag = _lastDamageInfo.Flags;
			corpse.Perks = new string[Perks.Count];

			for ( int i = 0; i < Perks.Count; i++ )
			{
				corpse.Perks[i] = Perks.Get( i ).Data.Title;
			}

			corpse.CopyFrom( this );
			corpse.ApplyForceToBone( force, forceBone );

			PlayerCorpse = corpse;
		}
	}
}
