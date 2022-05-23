using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string CorpseFound = "ttt.player.corpse-found";

		/// <summary>
		/// Occurs when a corpse has been found and confirmed.
		/// <para>Parameters:</para>
		/// <para>The <strong><see cref="TTT.Player"/></strong> whose corpse was found.</para>
		/// </summary>
		public class CorpseFoundAttribute : EventAttribute
		{
			public CorpseFoundAttribute() : base( CorpseFound ) { }
		}

		public const string Killed = "ttt.player.killed";

		/// <summary>
		/// Occurs when a player dies.
		/// <para>Parameters:</para>
		/// <para>The <strong><see cref="TTT.Player"/></strong> who died.</para>
		/// </summary>
		public class KilledAttribute : EventAttribute
		{
			public KilledAttribute() : base( Killed ) { }
		}

		public const string RoleChanged = "ttt.player.role-changed";

		/// <summary>
		/// Occurs when a player selects their role.
		/// <para>Parameters:</para>
		/// <para><strong><see cref="TTT.Player"/></strong> the player whose role has changed. </para>
		/// <para><strong><see cref="BaseRole"/></strong> their old role. </para>
		/// </summary>
		public class RoleChangedAttribute : EventAttribute
		{
			public RoleChangedAttribute() : base( RoleChanged ) { }
		}

		public const string Spawned = "ttt.player.spawned";

		/// <summary>
		/// Occurs when a player spawns.
		/// <para>Parameters:</para>
		/// <para><strong><see cref="TTT.Player"/></strong> who spawned.</para>
		/// </summary>
		public class SpawnedAttribute : EventAttribute
		{
			public SpawnedAttribute() : base( Spawned ) { }
		}
	}
}
