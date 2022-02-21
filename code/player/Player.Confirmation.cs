using Sandbox;

namespace TTT;

public partial class Player
{
	public new Corpse Corpse { get; set; }

	[Net]
	public int CorpseCredits { get; set; } = 0;

	public bool IsConfirmed { get; set; } = false;

	public bool IsMissingInAction { get; set; } = false;

	public void RemovePlayerCorpse()
	{
		if ( !Corpse.IsValid() )
			return;

		Corpse.Delete();
		Corpse = null;
	}

	private void BecomePlayerCorpseOnServer()
	{
		Corpse corpse = new()
		{
			Position = Position,
			Rotation = Rotation
		};

		corpse.CopyFrom( this );
		corpse.ApplyForceToBone( LastDamageInfo.Force, GetHitboxBone( LastDamageInfo.HitboxIndex ) );

		Corpse = corpse;
	}

	public void SyncMIA( Player player = null )
	{
		if ( Game.Current.Round is not InProgressRound )
			return;

		if ( player == null )
			AddMIA( Team.Traitors.ToClients() );
		else
			AddMIA( To.Single( player ) );
	}

	[ClientRpc]
	private void AddMIA()
	{
		IsMissingInAction = true;

		UI.Scoreboard.Instance.UpdateClient( Client );
	}
}
