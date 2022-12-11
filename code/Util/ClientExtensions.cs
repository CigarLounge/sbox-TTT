using System;
using Sandbox;

namespace TTT;

public static class ClientExtensions
{
	public static void Ban( this IClient client, int minutes = default, string reason = "" )
	{
		client.Kick();
		TTTGame.BannedClients.Add
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
