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
		/// <para><strong><see cref="TTT.BaseState"/></strong> the old state.</para>
		/// <para><strong><see cref="TTT.BaseState"/></strong> the new state.</para>
		/// </summary>
		public class StateChangedAttribute : EventAttribute
		{
			public StateChangedAttribute() : base( StateChanged ) { }
		}
	}
}
