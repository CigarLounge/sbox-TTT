using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class InfoFeed : Panel
{
	public static InfoFeed Instance;

	public InfoFeed()
	{
		Instance = this;
	}

	public void AddEntry( string method, Color? color = null )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();
		Label label = e.AddLabel( method, "method" );
		label.Style.FontColor = color ?? Color.White;
	}

	public void AddClientEntry( Client leftClient, string message )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;
		Player leftPlayer = leftClient.Pawn as Player;

		Label leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

		e.AddLabel( message, "method" );
	}

	public void AddRoleEntry( RoleInfo roleInfo, string interaction )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		Label leftLabel = e.AddLabel( $"{roleInfo.Title}s", "left" );
		leftLabel.Style.FontColor = roleInfo.Color;

		e.AddLabel( interaction, "method" );
	}

	public void AddClientToClientEntry( Client leftClient, string rightClientName, Color rightClientRoleColor, string method, string postfix = "" )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;
		bool isRightLocal = rightClientName == Local.Client.Name;

		Player leftPlayer = leftClient.Pawn as Player;

		Label leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

		e.AddLabel( method, "method" );

		Label rightLabel = e.AddLabel( isRightLocal ? "You" : rightClientName, "right" );
		rightLabel.Style.FontColor = rightClientRoleColor;

		if ( !string.IsNullOrEmpty( postfix ) )
			e.AddLabel( postfix, "append" );
	}

	[ClientRpc]
	public static void DisplayEntry( string message )
	{
		Instance?.AddEntry( message );
	}

	[ClientRpc]
	public static void DisplayEntry( string message, Color color )
	{
		Instance?.AddEntry( message, color );
	}

	[ClientRpc]
	public static void DisplayClientEntry( string message )
	{
		Instance?.AddClientEntry( Local.Client, message );
	}

	[ClientRpc]
	public static void DisplayRoleEntry( RoleInfo roleInfo, string message )
	{
		Instance?.AddRoleEntry( roleInfo, message );
	}
}
