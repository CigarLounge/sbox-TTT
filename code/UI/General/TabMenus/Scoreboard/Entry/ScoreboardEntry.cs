using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	public PlayerStatus PlayerStatus;
	private readonly Client _client;

	private Image PlayerAvatar { get; init; }
	private Label PlayerName { get; init; }
	private Label Karma { get; init; }
	private Label Score { get; init; }
	private Label Ping { get; init; }
	private Label Tag { get; init; }

	private bool Expanded = false;
	private List<string> _tags = new() { "Friend", "Suspect", "Avoid", "Kill", "Missing" };

	private void SetTag(string tag)
	{
		int i = 0;
		foreach (string cls in Tag.Class)
		{
			if (i >= 2)
				Tag.RemoveClass(cls);
			i++;
		}

		if (Tag.Text == tag)
			Tag.Text = "";
		else {
			Tag.Text = tag;
			Tag.AddClass(tag);
		}
	}

	public ScoreboardEntry( Panel parent, Client client ) : base( parent )
	{
		_client = client;

		// Add tag buttons below each entry
		foreach (string tag in _tags)
		{
			var btn = GetChild(1).Add.Button(tag, () => { SetTag(tag); });
			btn.AddClass(tag);
		}
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
		// Only allow tagging currently alive players, and only if the selected player is not the local player
		if ((_client.Pawn as Player).Status is not PlayerStatus.Alive || _client.Pawn.IsLocalPawn)
			return;

		if (!Expanded) {
			Style.Set("height", "76px");
			GetChild(1).RemoveClass("no-display");
			Expanded = true;
		}
		else {
			Style.Set("height", "38px");
			GetChild(1).AddClass("no-display");
			Expanded = false;
		}
	}

	[GameEvent.Round.PreRound]
	private void OnRoundStart()
	{
		SetTag("");
	}
}
