using Sandbox;

namespace TTT;

public static partial class GameEvent
{
	public static class Client
	{
		public const string Joined = "ttt.client.joined";

		/// <summary>
		/// Called everytime a player joins.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Sandbox.Client"/> that joined.</para>
		/// </summary>
		public class JoinedAttribute : EventAttribute
		{
			public JoinedAttribute() : base( Joined ) { }
		}

		public const string Disconnected = "ttt.client.disconnected";

		/// <summary>
		/// Called everytime a player leaves the game.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Sandbox.Client"/> that joined.</para>
		/// <para>The <see cref="NetworkDisconnectionReason"/></para>
		/// </summary>
		public class ClientDisconnectedAttribute : EventAttribute
		{
			public ClientDisconnectedAttribute() : base( Disconnected ) { }
		}
	}
}
