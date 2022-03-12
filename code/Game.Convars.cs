using Sandbox;

namespace TTT;

public partial class Game
{
	#region Round

	[ServerVar( "ttt_preround_time", Help = "The length of the preround time.", Saved = true )]
	public static int PreRoundTime { get; set; } = 20;

	[ServerVar( "ttt_inprogressround_time", Help = "The length of the in progress round time.", Saved = true )]
	public static int InProgressRoundTime { get; set; } = 420;

	[ServerVar( "ttt_postround_time", Help = "The length of the postround time.", Saved = true )]
	public static int PostRoundTime { get; set; } = 10;

	[ServerVar( "ttt_mapselection_time", Help = "The length of the map selection period.", Saved = true )]
	public static int MapSelectionTime { get; set; } = 10;

	[ServerVar( "ttt_round_limit", Help = "The max number of rounds until the maps is switched.", Saved = true )]
	public static int RoundLimit { get; set; } = 10;
	#endregion

	#region Debug
	[ServerVar( "ttt_round_debug", Help = "Stop the in progress round from ending.", Saved = true )]
	public static bool PreventWin { get; set; } = false;
	#endregion

	#region Map Related
	[ServerVar( "ttt_default_map", Help = "The default map to swap to if no maps are found.", Saved = true )]
	public static string DefaultMap { get; set; } = "facepunch.flatgrass";
	#endregion

	#region Minimum Players
	[ConVar.Replicated( "ttt_min_players", Help = "The minimum players to start the game.", Saved = true )]
	public static int MinPlayers { get; set; } = 2;
	#endregion

	#region AFK Timers
	[ServerVar( "ttt_afk_timer", Help = "The amount of time before a player is marked AFK.", Saved = true )]
	public static int AFKTimer { get; set; } = 180;

	[ServerVar( "ttt_afk_kick", Help = "Kick any players that get marked AFK.", Saved = true )]
	public static bool KickAFKPlayers { get; set; } = false;
	#endregion
}
