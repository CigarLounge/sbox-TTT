using Sandbox;

namespace TTT;

public static partial class NetworkedEvent
{
	/// <summary>
	/// Runs an event on the Server and Client.
	/// </summary>
	/// <param name="to"></param>
	/// <param name="name"></param>
	public static void Run( To to, string name )
	{
		Event.Run( name );
		RunClient( to, name );
	}

	[ClientRpc]
	public static void RunClient( string name )
	{
		Event.Run( name );
	}
}
