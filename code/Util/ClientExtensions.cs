using Sandbox;
using System;

namespace TTT;

public static class ClientExtensions
{
	public static void Ban( this IClient client, int minutes = default, string reason = "" )
	{
		client.Kick();
		GameManager.BannedClients.Add
		(
			new BannedClient
			{
				SteamId = client.SteamId,
				Duration = minutes == default ? DateTime.MaxValue : DateTime.Now.AddMinutes( minutes ),
				Reason = reason
			}
		);
	}

	public static bool HasRockedTheVote( this IClient client )
	{
		return client.GetValue<bool>( "!rtv" );
	}
}
