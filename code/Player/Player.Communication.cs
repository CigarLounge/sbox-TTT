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
	/// <summary>
	/// The current chat channel to send messages to.
	/// </summary>
	[ConVar.ClientData( "channel_current" )]
	public Channel CurrentChannel { get; set; } = Channel.Spectator;

	/// <summary>
	/// Determines which players are currently muted.
	/// </summary>
	[ConVar.ClientData( "mute_filter" )]
	public MuteFilter MuteFilter { get; set; } = MuteFilter.None;

	/// <summary>
	/// Clientside only.
	/// </summary>
	public ColorGroup TagGroup { get; set; }

	public bool CanHearSpectators => (!this.IsAlive() || Game.Current.State is not InProgress) && MuteFilter != MuteFilter.Spectators && MuteFilter != MuteFilter.All;
	public bool CanHearAlivePlayers => MuteFilter != MuteFilter.AlivePlayers && MuteFilter != MuteFilter.All;

	public string LastWords
	{
		get => _timeSinceLastWords < 3 ? _lastWords : string.Empty;
		set
		{
			_timeSinceLastWords = 0;
			_lastWords = value;
		}
	}
	private string _lastWords;
	private TimeSince _timeSinceLastWords;

	public static void ToggleMute()
	{
		var player = Local.Pawn as Player;

		if ( ++player.MuteFilter > MuteFilter.All )
			player.MuteFilter = MuteFilter.None;
	}
}
