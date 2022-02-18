namespace TTT.Gamemode;

public static class GameConVars
{
	[ServerVar( "ttt_min_players", Help = "The minimum players to start the game.", Saved = true )]
	public static int MinPlayers { get; set; } = 2;
}