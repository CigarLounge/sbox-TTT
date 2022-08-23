using Sandbox;

namespace TTT;

public enum Channel
{
	All,
	Team,
	Spectator
}

public enum MuteFilter
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

	[ConVar.ClientData( "mute_filter" )]
	public MuteFilter MuteFilter { get; set; } = MuteFilter.None;

	public bool CanHearSpectators => (!this.IsAlive() || Game.Current.State is not InProgress) && MuteFilter != MuteFilter.Spectators && MuteFilter != MuteFilter.All;
	public bool CanHearAlivePlayers => MuteFilter != MuteFilter.AlivePlayers && MuteFilter != MuteFilter.All;

	public static void ToggleMute()
	{
		var player = Local.Pawn as Player;

		if ( ++player.MuteFilter > MuteFilter.All )
			player.MuteFilter = MuteFilter.None;
	}
}
