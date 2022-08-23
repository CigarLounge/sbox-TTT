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

	public bool CanHearSpectators => !this.IsAlive() && (PlayersMuted != PlayersMute.Spectators || PlayersMuted != PlayersMute.All);
	public bool CanHearAlivePlayers => this.IsAlive() && (PlayersMuted != PlayersMute.AlivePlayers || PlayersMuted != PlayersMute.All);

	public static void ToggleMute()
	{
		var player = Local.Pawn as Player;

		player.PlayersMuted = player.PlayersMuted switch
		{
			PlayersMute.None => PlayersMute.AlivePlayers,
			PlayersMute.AlivePlayers => PlayersMute.Spectators,
			PlayersMute.Spectators => PlayersMute.All,
			PlayersMute.All => PlayersMute.None,
			_ => PlayersMute.None
		};
	}

	[GameEvent.Round.Start]
	private void UnmutePlayers()
	{
		if ( !Host.IsServer )
			return;

		PlayersMuted = PlayersMute.None;
	}
}
