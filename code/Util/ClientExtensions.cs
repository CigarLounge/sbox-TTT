using System;
using Sandbox;

namespace TTT;

public static class ClientExtensions
{
	public static void Ban( this Client client, int minutes = default, string reason = "" )
	{
		client.Kick();
		Game.BannedClients.Add
		(
			new BannedClient
			{
				SteamId = client.SteamId,
				Duration = minutes == default ? DateTime.MaxValue : DateTime.Now.AddMinutes( minutes ),
				Reason = reason
			}
		);
	}
}
