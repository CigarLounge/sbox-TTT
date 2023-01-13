using Sandbox;

namespace TTT;

public static partial class GameEvent
{
	public static class State
	{
		public const string Start = "ttt.state.start";

		/// <summary>
		/// Occurs when any state starts.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="BaseState"/> state that started. </para>
		/// </summary>
		public class StartAttribute : EventAttribute
		{
			public StartAttribute() : base( Start ) { }
		}

		public const string End = "ttt.state.end";

		/// <summary>
		/// Occurs when a state has ended.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="BaseState"/> that has just ended. </para>
		/// </summary>
		public class EndAttribute : EventAttribute
		{
			public EndAttribute() : base( End ) { }
		}
	}
}
