using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string CorpseFound = "ttt.player.corpse-found";

		/// <summary>
		/// Occurs when a corpse has been found and confirmed.
		/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> who's corpse was found .</para>
		/// </summary>
		public class CorpseFoundAttribute : EventAttribute
		{
			public CorpseFoundAttribute() : base( CorpseFound ) { }
		}

		public const string Killed = "ttt.player.killed";

		/// <summary>
		/// Occurs when a player dies.
		/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> who died.</para>
		/// </summary>
		public class KilledAttribute : EventAttribute
		{
			public KilledAttribute() : base( Killed ) { }
		}

		public const string RoleChanged = "ttt.player.role-changed";

		/// <summary>
		/// Occurs when a player selects their role.
		/// <para>Event is passed the <strong><see cref="TTT.Player"/></strong> whose role was changed
		/// and the previous <strong><see cref="TTT.BaseRole"/></strong>.</para>
		/// </summary>
		public class RoleChangedAttribute : EventAttribute
		{
			public RoleChangedAttribute() : base( RoleChanged ) { }
		}

	}
}
