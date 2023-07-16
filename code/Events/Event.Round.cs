using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Round
	{
		public const string Start = "ttt.round.start";

		/// <summary>
		/// Occurs when the roles have been assigned and the round has started.
		/// </summary>
		public class StartAttribute : EventAttribute
		{
			public StartAttribute() : base( Start ) { }
		}

		public const string End = "ttt.round.end";

		/// <summary>
		/// Occurs when a round has ended.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Team"/> that won the round. </para>
		/// <para>The <see cref="WinType"/>.</para>
		/// </summary>
		public class EndAttribute : EventAttribute
		{
			public EndAttribute() : base( End ) { }
		}
	}
}
