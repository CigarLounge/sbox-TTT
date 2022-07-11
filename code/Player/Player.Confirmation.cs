using Sandbox;

namespace TTT;

public enum SomeState
{
	Alive,
	MissingInAction,
	ConfirmedDead,
	Spectator
}

public partial class Player
{
	public Corpse Corpse { get; set; }

	/// <summary>
	/// The player who confirmed this player's corpse.
	/// </summary>
	public Player Confirmer { get; private set; }

	private SomeState _someState;
	public SomeState SomeState
	{
		get => _someState;
		set
		{
			if ( _someState == value )
				return;

			_someState = value;

			if ( IsServer && value == SomeState.Spectator )
			{
				Client.SetValue( Strings.Spectator, true );
				SetSomeState( SomeState.Spectator );
			}
		}
	}
	public bool IsMissingInAction => SomeState == SomeState.MissingInAction;
	public bool IsConfirmedDead => SomeState == SomeState.ConfirmedDead;
	public bool IsRoleKnown { get; set; }
	public string LastSeenPlayerName { get; set; }

	public void RemoveCorpse()
	{
		Host.AssertServer();

		if ( !Corpse.IsValid() )
			return;

		Corpse.Delete();
		Corpse = null;
	}

	private void BecomeCorpse()
	{
		Host.AssertServer();

		Corpse = new Corpse( this );
	}

	public void UpdateMissingInAction( Player player = null )
	{
		Host.AssertServer();

		if ( player is not null )
		{
			SetSomeState( To.Single( player ), SomeState.MissingInAction );
			return;
		}

		SomeState = SomeState.MissingInAction;
		SetSomeState( Team.Traitors.ToClients(), SomeState.MissingInAction );
	}

	public void Confirm( To to, Player confirmer = null )
	{
		Host.AssertServer();

		var wasPreviouslyConfirmed = true;

		if ( !IsConfirmedDead )
		{
			Confirmer = confirmer;
			SomeState = SomeState.ConfirmedDead;
			IsRoleKnown = true;
			wasPreviouslyConfirmed = false;
		}

		if ( Corpse.IsValid() )
			Corpse.SendPlayer( to );

		SendRole( to );
		ClientConfirm( to, Confirmer, wasPreviouslyConfirmed );
	}

	private void CheckLastSeenPlayer()
	{
		if ( HoveredEntity is Player player && player.CanHint( this ) )
			LastSeenPlayerName = player.Client.Name;
	}

	private void ResetConfirmationData()
	{
		Confirmer = null;
		Corpse = null;
		LastSeenPlayerName = string.Empty;
		SomeState = SomeState.Alive;
		IsRoleKnown = false;
	}

	[ClientRpc]
	private void ClientConfirm( Player confirmer, bool wasPreviouslyConfirmed = false )
	{
		Confirmer = confirmer;
		SomeState = SomeState.ConfirmedDead;

		if ( wasPreviouslyConfirmed || !Confirmer.IsValid() || !Corpse.IsValid() )
			return;

		Event.Run( TTTEvent.Player.CorpseFound, this );
	}

	[ClientRpc]
	private void SetSomeState( SomeState someState )
	{
		SomeState = someState;
	}
}
