using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Game
	{
		public const string ClientJoined = "ttt.game.client-joined";

		/// <summary>
		/// Called everytime a player joins.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Client"/> that joined.</para>
		/// </summary>
		public class ClientJoinedAttribute : EventAttribute
		{
			public ClientJoinedAttribute() : base( ClientJoined ) { }
		}

		public const string ClientDisconnected = "ttt.game.client-disconnected";

		/// <summary>
		/// Called everytime a player leaves the game.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Client"/> that joined.</para>
		/// <para>The <see cref="NetworkDisconnectionReason"/></para>
		/// </summary>
		public class ClientDisconnectedAttribute : EventAttribute
		{
			public ClientDisconnectedAttribute() : base( ClientDisconnected ) { }
		}

		public const string StateChanged = "ttt.game.state-changed";

		/// <summary>
		/// Called everytime the game state changes.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The old <see cref="BaseState"/>.</para>
		/// <para>The new <see cref="BaseState"/>.</para>
		/// </summary>
		public class StateChangedAttribute : EventAttribute
		{
			public StateChangedAttribute() : base( StateChanged ) { }
		}
	}
}
