using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Settings
	{
		public const string Changed = "ttt.settings.changed";

		/// <summary>
		/// Occurs when server or client settings are changed.
		/// <para>No data is passed to this event.</para>
		/// </summary>
		public class ChangedAttribute : EventAttribute
		{
			public ChangedAttribute() : base( Changed ) { }
		}
	}
}
