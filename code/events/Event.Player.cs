using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string Died = "ttt.player.died";

		/// <summary>
		/// Occurs when a player dies.
		/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> instance of the player who died.</para>
		/// </summary>
		public class DiedAttribute : EventAttribute
		{
			public DiedAttribute() : base( Died ) { }
		}

		public static class Role
		{
			/// <summary>
			/// Occurs when a player selects their role.
			/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> instance of the player whose role was set.</para>
			/// </summary>
			public const string Changed = "ttt.player.role.changed";

			public class ChangedAttribute : EventAttribute
			{
				public ChangedAttribute() : base( Changed ) { }
			}
		}
	}
}
