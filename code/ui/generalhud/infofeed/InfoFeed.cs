using Sandbox;
using Sandbox.UI;

using TTT.Player;
using TTT.Roles;

namespace TTT.UI;

public partial class InfoFeed : Panel
{
	public static InfoFeed Current;

	public InfoFeed() : base()
	{
		Current = this;

		StyleSheet.Load( "/ui/generalhud/infofeed/InfoFeed.scss" );
	}

	public virtual Panel AddEntry( Client leftClient, string method )
	{
		InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;

		TTTPlayer leftPlayer = leftClient.Pawn as TTTPlayer;

		Label leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Info.Color;

		e.AddLabel( method, "method" );

		return e;
	}

	public virtual Panel AddEntry( string method, Color? color = null )
	{
		InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

		Label label = e.AddLabel( method, "method" );
		label.Style.FontColor = color ?? Color.White;

		return e;
	}

	public virtual Panel AddEntry( Client leftClient, string rightClientName, Color rightClientRoleColor, string method, string postfix = "" )
	{
		InfoFeedEntry e = Current.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;
		bool isRightLocal = rightClientName == Local.Client.Name;

		TTTPlayer leftPlayer = leftClient.Pawn as TTTPlayer;

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
