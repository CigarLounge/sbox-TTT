using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Game
	{
		public const string StateChanged = "ttt.game.state-changed";

		/// <summary>
		/// Called everytime the game state changes.
		/// <para>Parameters:</para>
		/// <para>The old <strong><see cref="BaseState"/></strong>.</para>
		/// <para>The new <strong><see cref="BaseState"/></strong>.</para>
		/// </summary>
		public class StateChangedAttribute : EventAttribute
		{
			public StateChangedAttribute() : base( StateChanged ) { }
		}
	}
}
