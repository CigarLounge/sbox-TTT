using System.Collections.Generic;
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
	/// <summary>
	/// The current chat channel to send messages to.
	/// </summary>
	[ConVar.ClientData( "channel_current" )]
	public Channel CurrentChannel { get; set; } = Channel.Spectator;

	/// <summary>
	/// Determines which players are currently muted.
	/// </summary>
	[ConVar.ClientData( "muted_players" )]
	public PlayersMute PlayersMuted { get; set; } = PlayersMute.None;

	public bool CanHearSpectators => (!this.IsAlive() || Game.Current.State is not InProgress) && PlayersMuted != PlayersMute.Spectators && PlayersMuted != PlayersMute.All;
	public bool CanHearAlivePlayers => PlayersMuted != PlayersMute.AlivePlayers && PlayersMuted != PlayersMute.All;

	/// <summary>
	/// Each tag (i.e "Friend") that is associated with a player.
	/// </summary>
	public static readonly Dictionary<Client, ColorGroup> TagCollection = new();

	public static readonly ColorGroup[] TagGroups = new ColorGroup[]
	{
		new ColorGroup("Friend", Color.FromBytes(0, 255, 0)),
		new ColorGroup("Suspect", Color.FromBytes(179, 179, 20)),
		new ColorGroup("Missing", Color.FromBytes(130, 190, 130)),
		new ColorGroup("Kill", Color.FromBytes(255, 0, 0))
	};

	public static void ToggleMute()
	{
		var player = Local.Pawn as Player;

		if ( ++player.PlayersMuted > PlayersMute.All )
			player.PlayersMuted = PlayersMute.None;
	}
}
