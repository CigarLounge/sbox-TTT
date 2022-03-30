using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class InfoFeed : Panel
{
	public static InfoFeed Instance;

	public InfoFeed() : base()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/InfoFeed/InfoFeed.scss" );
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

	public void AddRoleEntry( int roleId, string interaction )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		var roleInfo = Asset.FromId<RoleInfo>( roleId );
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
}
