using Sandbox;

namespace TTT;

public enum Channel
{
	All,
	Team,
	Spectator
}

public enum PlayersMute
{
	None,
	AlivePlayers,
	Spectators,
	All
}

public partial class Player
{
	[ConVar.ClientData( "channel_current" )]
	public Channel CurrentChannel { get; set; } = Channel.Spectator;

	[ConVar.ClientData( "muted_players" )]
	public PlayersMute PlayersMuted { get; set; } = PlayersMute.None;

	public bool CanHearSpectators => (!this.IsAlive() || Game.Current.State is not InProgress) && (PlayersMuted != PlayersMute.Spectators || PlayersMuted != PlayersMute.All);
	public bool CanHearAlivePlayers => this.IsAlive() && (PlayersMuted != PlayersMute.AlivePlayers || PlayersMuted != PlayersMute.All);

	public static void ToggleMute()
	{
		var player = Local.Pawn as Player;

		if ( ++player.PlayersMuted > PlayersMute.All )
			player.PlayersMuted = PlayersMute.None;
	}

	[GameEvent.Round.Start]
	private void UnmutePlayers()
	{
		if ( !Host.IsServer )
			return;

		PlayersMuted = PlayersMute.None;
	}
}
