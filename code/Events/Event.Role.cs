using Sandbox;

namespace TTT;

public static partial class GameEvent
{
	public static class Role
	{
		public const string AllRolesAssigned = "ttt.role.all-assigned";

		public class AllRolesAssignedAttribute : EventAttribute
		{
			public AllRolesAssignedAttribute() : base( AllRolesAssigned ) { }
		}
	}
}
