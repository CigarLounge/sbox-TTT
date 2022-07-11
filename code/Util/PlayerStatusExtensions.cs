namespace TTT;

public static class PlayerStatusExtensions
{
	public static Color GetColor( this PlayerStatus status )
	{
		return status switch
		{
			PlayerStatus.Alive => Color.FromBytes( 23, 173, 68 ),
			PlayerStatus.MissingInAction => Color.FromBytes( 80, 117, 79 ),
			PlayerStatus.ConfirmedDead => Color.FromBytes( 198, 91, 58 ),
			PlayerStatus.Spectator => Color.FromBytes( 252, 219, 56 ),
			_ => string.Empty,
		};
	}

	public static string ToStringFast( this PlayerStatus status )
	{
		return status switch
		{
			PlayerStatus.Alive => "Alive",
			PlayerStatus.MissingInAction => "Missing In Action",
			PlayerStatus.ConfirmedDead => "Confirmed Dead",
			PlayerStatus.Spectator => "Spectator",
			_ => string.Empty,
		};
	}
}
