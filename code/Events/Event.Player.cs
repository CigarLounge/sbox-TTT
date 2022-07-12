using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Player
	{
		public const string CorpseFound = "ttt.player.corpse-found";

		/// <summary>
		/// Occurs when a corpse has been found and confirmed.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> whose corpse was found.</para>
		/// </summary>
		public class CorpseFoundAttribute : EventAttribute
		{
			public CorpseFoundAttribute() : base( CorpseFound ) { }
		}

		public const string Killed = "ttt.player.killed";

		/// <summary>
		/// Occurs when a player dies.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> who died.</para>
		/// </summary>
		public class KilledAttribute : EventAttribute
		{
			public KilledAttribute() : base( Killed ) { }
		}

		public const string RoleChanged = "ttt.player.role-changed";

		/// <summary>
		/// Occurs when a player's role has changed.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> whose role has changed. </para>
		/// <para><see cref="Role"/> their old role. </para>
		/// </summary>
		public class RoleChangedAttribute : EventAttribute
		{
			public RoleChangedAttribute() : base( RoleChanged ) { }
		}

		public const string Spawned = "ttt.player.spawned";

		/// <summary>
		/// Occurs when a player spawns.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> who spawned.</para>
		/// </summary>
		public class SpawnedAttribute : EventAttribute
		{
			public SpawnedAttribute() : base( Spawned ) { }
		}

		public const string StatusChanged = "ttt.player.status-changed";

		/// <summary>
		/// Occurs when a player's status has changed.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> whose status has changed.</para>
		/// <para>The old <see cref="PlayerStatus"/>.</para>
		/// </summary>
		public class StatusChangedAttribute : EventAttribute
		{
			public StatusChangedAttribute() : base( StatusChanged ) { }
		}

		public const string TookDamage = "ttt.player.took-damage";

		/// <summary>
		/// Occurs when a player takes damage.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="TTT.Player"/> who was damaged.</para>
		/// </summary>
		public class TookDamageAttribute : EventAttribute
		{
			public TookDamageAttribute() : base( TookDamage ) { }
		}
	}
}
