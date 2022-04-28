using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	public string ScoreboardGroupName;
	public Client Client;

	private Image PlayerAvatar { get; set; }
	private Label PlayerName { get; set; }
	private Label Karma { get; set; }
	private Label Score { get; set; }
	private Label Ping { get; set; }

	public virtual void Update()
	{
		if ( Client is null )
			return;

		if ( Client.Pawn is not Player player )
			return;

		PlayerName.Text = Client.Name;

		Karma.Enabled( Game.KarmaEnabled );
		if ( Karma.IsEnabled() )
			Karma.Text = MathF.Round( player.BaseKarma ).ToString();

		Ping.Text = Client.Ping.ToString();
		Score.Text = player.Score.ToString();

		SetClass( "me", Client == Local.Client );

		if ( player.Role is not NoneRole and not Innocent )
			Style.BackgroundColor = player.Role.Color.WithAlpha( 0.15f );
		else
			Style.BackgroundColor = null;

		PlayerAvatar.SetTexture( $"avatar:{Client.PlayerId}" );
	}
}
