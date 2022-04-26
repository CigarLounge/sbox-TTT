using Sandbox;

namespace TTT;

public partial class Game
{
	#region Round
	[ServerVar( "ttt_preround_time", Help = "The length of the preround time.", Saved = true )]
	public static int PreRoundTime { get; set; } = 20;

	[ServerVar( "ttt_inprogressround_time", Help = "The length of the in progress round time.", Saved = true )]
	public static int InProgressRoundTime { get; set; } = 300;

	[ServerVar( "ttt_inprogressround_secs_per_death", Help = "The number of seconds to add to the in progress round timer when someone dies.", Saved = true )]
	public static int InProgressSecondsPerDeath { get; set; } = 15;

	[ServerVar( "ttt_postround_time", Help = "The length of the postround time.", Saved = true )]
	public static int PostRoundTime { get; set; } = 10;

	[ServerVar( "ttt_mapselection_time", Help = "The length of the map selection period.", Saved = true )]
	public static int MapSelectionTime { get; set; } = 10;
	#endregion

	#region Debug
	[ServerVar( "ttt_round_debug", Help = "Stop the in progress round from ending.", Saved = true )]
	public static bool PreventWin { get; set; } = false;
	#endregion

	#region Karma
	[ServerVar( "ttt_karma_enabled", Help = "Whether or not the karma system is enabled.", Saved = true )]
	public static bool KarmaEnabled { get; set; } = true;

	[ServerVar( "ttt_karma_low_autokick", Help = "Whether or not to kick a player with low karma.", Saved = true )]
	public static bool KarmaLowAutoKick { get; set; } = true;
	#endregion

	#region Map
	[ServerVar( "ttt_default_map", Help = "The default map to swap to if no maps are found.", Saved = true )]
	public static string DefaultMap { get; set; } = "facepunch.flatgrass";

	[ServerVar( "ttt_rtv_threshold", Help = "The percentage of players needed to RTV.", Saved = true )]
	public static float RTVThreshold { get; set; } = 0.66f;

	[ConVar.Replicated( "ttt_round_limit", Help = "The maximum amount of rounds that can be played.", Saved = true )]
	public static int RoundLimit { get; set; } = 6;
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

	#region Credits
	[ServerVar( "ttt_credits_award_pct", Help = "When this percentage of Innocents are dead, Traitors are given credits.", Saved = true )]
	public static float CreditsAwardPercentage { get; set; } = 0.35f;


	[ServerVar( "ttt_credits_traitordeath", Help = "The number of credits Detectives receive when a Traitor dies.", Saved = true )]
	public static int DetectiveTraitorDeathReward { get; set; } = 100;

	[ServerVar( "ttt_credits_detectivekill", Help = "The number of credits a Traitor receives when they kill a Detective.", Saved = true )]
	public static int TraitorDetectiveKillReward { get; set; } = 100;
	#endregion
}
