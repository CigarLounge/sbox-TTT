using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Shop
	{
		public const string Changed = "ttt.shop.changed";

		/// <summary>
		/// Occurs when the shop is changed.
		/// <para>No data is passed to this event.</para>
		/// </summary>
		public class ChangedAttribute : EventAttribute
		{
			public ChangedAttribute() : base( Changed ) { }
		}
	}
}
