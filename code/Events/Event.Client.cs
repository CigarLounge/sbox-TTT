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
		/// <para>The <see cref="Sandbox.IClient"/> that joined.</para>
		/// </summary>
		public class JoinedAttribute : EventAttribute
		{
			public JoinedAttribute() : base( Joined ) { }
		}

		public const string Disconnected = "ttt.client.disconnected";

		/// <summary>
		/// Called everytime a player leaves the game.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Sandbox.IClient"/> that disconnected.</para>
		/// </summary>
		public class DisconnectedAttribute : EventAttribute
		{
			public DisconnectedAttribute() : base( Disconnected ) { }
		}
	}
}
