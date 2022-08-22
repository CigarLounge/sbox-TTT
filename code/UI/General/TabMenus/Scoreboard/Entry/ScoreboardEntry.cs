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
	// private Button FriendButton { get; init; }
	// private Button SuspectButton { get; init; }
	// private Button AvoidButton { get; init; }
	// private Button KillButton { get; init; }
	// private Button MissingButton { get; init; }

	// private List<Button> Buttons;
	// private List<string> _tags = new() { "Friend", "Suspect", "Avoid", "Kill", "Missing" };
	// private List<string> _tags;


	private void SetTag(string tag)
	{
		Tag.Text = tag;
		Tag.AddClass(tag);
	}

	public ScoreboardEntry( Panel parent, Client client ) : base( parent )
	{
		_client = client;
		GetChild(1).Add.Button( "Friend", () =>  { SetTag( "Friend" ); } );
		GetChild(1).Add.Button( "Suspect", () => { SetTag( "Suspect" ); } );
		GetChild(1).Add.Button( "Avoid", () =>   { SetTag( "Avoid" ); } );
		GetChild(1).Add.Button( "Kill", () =>    { SetTag( "Kill" ); } );
		GetChild(1).Add.Button( "Missing", () => { SetTag( "Missing" ); } );
		Karma.SetProperty("color", "red");
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

		Log.Info(GetChild(1));
	}

	public void OnClick()
	{
		HandleExpand();
	}

	private void HandleExpand()
	{
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
}
