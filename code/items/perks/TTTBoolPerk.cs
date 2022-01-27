namespace TTT.Items
{
	[Hammer.Skip]
	public abstract class TTTBoolPerk : TTTPerk
	{
		public abstract bool IsEnabled { get; set; }
	}
}
