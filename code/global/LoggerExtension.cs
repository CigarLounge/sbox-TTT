using Sandbox;

namespace TTT;

public static class LoggerExtension
{
	public static void Debug( this Logger log, object obj = null )
	{
		if ( !Game.Current.Debug )
			return;

		string host = Host.IsServer ? "SERVER" : "CLIENT";

		log.Info( $"[DEBUG][{host}] {obj}" );
	}
}
