using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string CorpseFound = "ttt.player.corpse-found";

		/// <summary>
		/// Occurs when a corpse has been found and confirmed.
		/// <para><strong><see cref="TTT.Player"/></strong> the player whose corpse was found.</para>
		/// </summary>
		public class CorpseFoundAttribute : EventAttribute
		{
			public CorpseFoundAttribute() : base( CorpseFound ) { }
		}

		public const string Killed = "ttt.player.killed";

		/// <summary>
		/// Occurs when a player dies.
		/// <para><strong><see cref="TTT.Player"/></strong> the player who died.</para>
		/// </summary>
		public class KilledAttribute : EventAttribute
		{
			public KilledAttribute() : base( Killed ) { }
		}

		public const string RoleChanged = "ttt.player.role-changed";

		/// <summary>
		/// Occurs when a player selects their role.
		/// <para><strong><see cref="TTT.Player"/></strong> the player whose role has changed. </para>
		/// <para><strong><see cref="TTT.BaseRole"/></strong> their old role. </para>
		/// </summary>
		public class RoleChangedAttribute : EventAttribute
		{
			public RoleChangedAttribute() : base( RoleChanged ) { }
		}

		public const string CreditsFound = "ttt.player.credits-found";

		/// <summary>
		/// Occurs when a player finds credits on a corpse.
		/// <para><strong><see cref="TTT.Player"/></strong> the player who found the credits on the corpse. </para>
		/// <para><strong><see cref="int"/></strong> credits that were found on the corpse.</para>
		/// </summary>
		public class CreditsFoundAttribute : EventAttribute
		{
			public CreditsFoundAttribute() : base( CreditsFound ) { }
		}
	}
}
