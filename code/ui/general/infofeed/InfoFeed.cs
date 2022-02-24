using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class InfoFeed : Panel
{
	public static InfoFeed Instance;

	public InfoFeed() : base()
	{
		Instance = this;

		StyleSheet.Load( "/ui/general/infofeed/InfoFeed.scss" );
	}

	public virtual Panel AddEntry( Client leftClient, string method )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;

		Player leftPlayer = leftClient.Pawn as Player;

		Label leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Info.Color;

		e.AddLabel( method, "method" );

		return e;
	}

	public virtual Panel AddEntry( string method, Color? color = null )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		Label label = e.AddLabel( method, "method" );
		label.Style.FontColor = color ?? Color.White;

		return e;
	}

	public virtual Panel AddEntry( Client leftClient, string rightClientName, Color rightClientRoleColor, string method, string postfix = "" )
	{
		InfoFeedEntry e = Instance.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;
		bool isRightLocal = rightClientName == Local.Client.Name;

		Player leftPlayer = leftClient.Pawn as Player;

		Label leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Info.Color;

		e.AddLabel( method, "method" );

		Label rightLabel = e.AddLabel( isRightLocal ? "You" : rightClientName, "right" );
		rightLabel.Style.FontColor = rightClientRoleColor;

		if ( !string.IsNullOrEmpty( postfix ) )
		{
			e.AddLabel( postfix, "append" );
		}

		return e;
	}
}
