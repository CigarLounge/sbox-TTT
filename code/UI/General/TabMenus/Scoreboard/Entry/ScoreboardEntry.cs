using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	public PlayerStatus PlayerStatus;
	public bool Expanded = false;
	private readonly Client _client;

	private Image PlayerAvatar { get; init; }
	private Label PlayerName { get; init; }
	private Label Karma { get; init; }
	private Label Score { get; init; }
	private Label Ping { get; init; }
	private Label Tag { get; init; }

	private ScoreboardEntryTagger EntryTagger { get; set; }

	public ScoreboardEntry( Panel parent, Client client ) : base( parent )
	{
		_client = client;
	}

	public void Update()
	{
		if ( _client.Pawn is not Player player )
			return;

		PlayerName.Text = _client.Name;

		Karma.Enabled( Game.KarmaEnabled );

		if ( Karma.IsEnabled() )
			Karma.Text = MathF.Round( player.BaseKarma ).ToString();

		Ping.Text = _client.IsBot ? "BOT" : _client.Ping.ToString();
		Score.Text = player.Score.ToString();
		Tag.Text = "Test";

		SetClass( "me", _client == Local.Client );

		if ( player.Role is not NoneRole and not Innocent )
			Style.BackgroundColor = player.Role.Color.WithAlpha( 0.15f );
		else
			Style.BackgroundColor = null;

		PlayerAvatar.SetTexture( $"avatar:{_client.PlayerId}" );
	}

	public void OnClick()
	{
		HandleExpand();
	}

	private void HandleExpand()
	{
		if (!Expanded) {
			Style.Set("height", "76px");
			EntryTagger = new ScoreboardEntryTagger(this, _client);

			// AddChild(EntryTagger);
			Parent.AddChild(EntryTagger);
			Expanded = true;
		}
		else {
			Style.Set("height", "38px");
			EntryTagger.Delete(true);
			Expanded = false;
		}
	}
}
