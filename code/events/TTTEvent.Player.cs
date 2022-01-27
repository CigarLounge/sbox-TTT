namespace TTT.Events
{
	public static partial class TTTEvent
	{
		public static class Player
		{
			/// <summary>
			/// Occurs when a player dies.
			/// <para>Event is passed the <strong><see cref="TTT.Player.TTTPlayer"/></strong> instance of the player who died.</para>
			/// </summary>
			public const string Died = "TTT.player.died";

			/// <summary>
			/// Occurs when a player initializes.
			/// <para>The <strong><see cref="Sandbox.Client"/></strong> instance of the player who spawned initially.</para>
			/// </summary>
			public const string InitialSpawn = "TTT.player.initialspawn";

			/// <summary>
			/// Occurs when a player spawns.
			/// <para>Event is passed the <strong><see cref="TTT.Player.TTTPlayer"/></strong> instance of the player spawned.</para>
			/// </summary>
			public const string Spawned = "TTT.player.spawned";

			/// <summary>
			/// Occurs when a player connects.
			/// <para>The <strong><see cref="Sandbox.Client"/></strong> instance of the player who connected.</para>
			/// </summary>
			public const string Connected = "TTT.player.connected";

			/// <summary>
			/// Occurs when a player disconnects.
			/// <para>The <strong><see cref="ulong"/></strong> of the player's PlayerId who disconnected.</para>
			/// <para>The <strong><see cref="Sandbox.NetworkDisconnectionReason"/></strong>.</para>
			/// </summary>
			public const string Disconnected = "TTT.player.disconnected";

			/// <summary>
			/// Occurs when a player takes damage.
			/// <para>The <strong><see cref="TTT.Player.TTTPlayer"/></strong> instance of the player who took damage.</para>
			/// <para>The <strong><see cref="float"/></strong> of the amount of damage taken.</para>
			/// </summary>
			public const string TakeDamage = "TTT.player.takedamage";

			public static class Inventory
			{
				/// <summary>
				/// Occurs when the player's inventory is cleared.
				/// <para>No data is passed to this event.</para>
				/// </summary>
				public const string Clear = "TTT.player.inventory.clear";

				/// <summary>
				/// Occurs when an item is dropped.
				/// <para>Event is passed the <strong><see cref="Items.ICarriableItem"/></strong> instance of the item dropped.</para>
				/// </summary>
				public const string Drop = "TTT.player.inventory.drop";

				/// <summary>
				/// Occurs when an item is picked up.
				/// <para>Event is passed the <strong><see cref="Items.ICarriableItem"/></strong> instance of the item picked up.</para>
				/// </summary>
				public const string PickUp = "TTT.player.inventory.pickup";
			}

			public static class Role
			{
				/// <summary>
				/// Occurs when a player selects their role.
				/// <para>Event is passed the <strong><see cref="TTT.Player.TTTPlayer"/></strong> instance of the player whose role was set.</para>
				/// </summary>
				public const string Select = "TTT.player.role.select";
			}

			public static class Spectating
			{
				/// <summary>
				/// Occurs when the player is changed to spectate.
				/// <para>Event is passed the <strong><see cref="TTT.Player.TTTPlayer"/></strong> instance of the player who was changed to spectate.</para>
				/// </summary>
				public const string Change = "TTT.player.spectating.change";
			}
		}
	}
}
