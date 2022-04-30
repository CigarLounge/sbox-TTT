using Sandbox;

namespace TTT;

public static partial class TTTEvent
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
		/// <para>Event is passed the winning <strong><see cref="TTT.Team"/></strong>
		/// and the <strong><see cref="TTT.WinType"/></strong>.</para>
		/// </summary>
		public class EndedAttribute : EventAttribute
		{
			public EndedAttribute() : base( Ended ) { }
		}
	}
}
