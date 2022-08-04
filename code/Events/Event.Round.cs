using Sandbox;

namespace TTT;

public static partial class GameEvent
{
	public static class Round
	{
		public const string Started = "ttt.round.started";

		/// <summary>
		/// Occurs when a new round starts.
		/// </summary>
		public class StartedAttribute : EventAttribute
		{
			public StartedAttribute() : base( Started ) { }
		}

		public const string RolesAssigned = "ttt.round.roles-assigned";

		/// <summary>
		/// Occurs when the roles have been assigned to each player.
		/// </summary>
		public class RolesAssignedAttribute : EventAttribute
		{
			public RolesAssignedAttribute() : base( RolesAssigned ) { }
		}

		public const string Ended = "ttt.round.ended";

		/// <summary>
		/// Occurs when a round has ended.
		/// <para><strong>Parameters:</strong></para>
		/// <para>The <see cref="Team"/> that won the round. </para>
		/// <para>The <see cref="WinType"/>.</para>
		/// </summary>
		public class EndedAttribute : EventAttribute
		{
			public EndedAttribute() : base( Ended ) { }
		}
	}
}
