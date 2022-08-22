using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	public PlayerStatus PlayerStatus;
	private readonly Client _client;

	private Image PlayerAvatar { get; init; }
	private Label PlayerName { get; init; }
	private Panel TagPanel { get; init; }
	private Label Tag { get; set; }
	private Label Karma { get; init; }
	private Label Score { get; init; }
	private Label Ping { get; init; }

	private Panel TagButtons { get; init; }

	private bool _isExpanded = false;
	private readonly static List<string> _tags = new() { "friend", "suspect", "avoid", "kill", "missing" };

	public ScoreboardEntry( Panel parent, Client client ) : base( parent )
	{
		_client = client;

		foreach (var tag in _tags)
		{
			var btn = TagButtons.Add.Button(tag.FirstCharToUpper(), () => { SetTag(tag); });
			btn.AddClass(tag);
		}

		TagButtons.Enabled(false);
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

		SetClass( "me", _client == Local.Client );

		if ( player.Role is not NoneRole and not Innocent )
			Style.BackgroundColor = player.Role.Color.WithAlpha( 0.15f );
		else
			Style.BackgroundColor = null;

		PlayerAvatar.SetTexture( $"avatar:{_client.PlayerId}" );
	}

	public void OnClick()
	{
		if ((_client.Pawn as Player).Status is not PlayerStatus.Alive || _client.Pawn.IsLocalPawn)
			return;

		if (TagButtons.IsEnabled())
		{
			Style.Set("height", "38px");
			TagButtons.Enabled(false);
		}
		else
		{
			Style.Set("height", "76px");
			TagButtons.Enabled(true);
		}
	}

	private void SetTag(string tag)
	{
		Tag.Delete();
		if (tag == string.Empty || Tag.Text == tag.FirstCharToUpper())
		{
			Tag.Text = string.Empty;
			return;
		}

		Tag = TagPanel.Add.Label(tag.FirstCharToUpper(), tag);
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		SetTag(string.Empty);
	}
}
