namespace TTT;

public static class SomeStateExtensions
{
	public static Color GetColor( this SomeState someState )
	{
		return someState switch
		{
			SomeState.Alive => Color.FromBytes( 23, 173, 68 ),
			SomeState.MissingInAction => Color.FromBytes( 80, 117, 79 ),
			SomeState.ConfirmedDead => Color.FromBytes( 198, 91, 58 ),
			SomeState.Spectator => Color.FromBytes( 252, 219, 56 ),
			_ => string.Empty,
		};
	}

	public static string ToStringFast( this SomeState someState )
	{
		return someState switch
		{
			SomeState.Alive => "Alive",
			SomeState.MissingInAction => "Missing In Action",
			SomeState.ConfirmedDead => "Confirmed Dead",
			SomeState.Spectator => "Spectator",
			_ => string.Empty,
		};
	}
}
