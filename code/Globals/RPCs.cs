using Sandbox;

namespace TTT;

public partial class RPCs
{
	[ClientRpc]
	public static void ClientDisplayEntry( string message, Color color )
	{
		UI.InfoFeed.Instance?.AddEntry( message, color );
	}

	[ClientRpc]
	public static void ClientDisplayClientEntry( string message )
	{
		UI.InfoFeed.Instance?.AddClientEntry( Local.Client, message );
	}

	[ClientRpc]
	public static void ClientDisplayRoleEntry( RoleInfo roleInfo, string message )
	{
		UI.InfoFeed.Instance?.AddRoleEntry( roleInfo, message );
	}
}
