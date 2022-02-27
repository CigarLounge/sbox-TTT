using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class UI
	{
		public const string Reloaded = "ttt.ui.reloaded";

		/// <summary>
		/// Occurs when the UI was reloaded.
		/// <para>No data is passed to this event.</para>
		/// </summary>
		public class ReloadedAttribute : EventAttribute
		{
			public ReloadedAttribute() : base( Reloaded ) { }
		}
	}
}
