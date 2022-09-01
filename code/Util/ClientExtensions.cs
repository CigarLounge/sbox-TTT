using Sandbox;

namespace TTT;

public static class ClientExtensions
{
	public static void Ban( this Client client )
	{
		client.SetValue( Strings.Karma, 0 );
		client.Kick();
	}
}
