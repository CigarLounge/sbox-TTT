using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string Killed = "ttt.player.killed";

		/// <summary>
		/// Occurs when a player dies.
		/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> instance of the player who died.</para>
		/// </summary>
		public class KilledAttribute : EventAttribute
		{
			public KilledAttribute() : base( Killed ) { }
		}

		public static class Role
		{
			public const string Changed = "ttt.player.role.changed";

			/// <summary>
			/// Occurs when a player selects their role.
			/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> instance of the player whose role was set
			/// and the <strong><see cref="TTT.BaseRole"/></strong> instance of the previous Role.</para>
			/// </summary>
			public class ChangedAttribute : EventAttribute
			{
				public ChangedAttribute() : base( Changed ) { }
			}
		}
	}
}
