using Sandbox;

namespace TTT;

public static partial class GameEvent
{
	public static class Roles
	{
		public const string Assigned = "ttt.role.assigned";

		/// <summary>
		/// Fired when all roles have been assigned.
		/// </summary>
		public class AssignedAttribute : EventAttribute
		{
			public AssignedAttribute() : base( Assigned ) { }
		}
	}
}
