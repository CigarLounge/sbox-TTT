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
	private Label Tag { get; set; }
	private Label Karma { get; init; }
	private Label Score { get; init; }
	private Label Ping { get; init; }

	private Panel TagButtons { get; init; }

	public ScoreboardEntry( Panel parent, Client client ) : base( parent )
	{
		_client = client;

		foreach ( var tagGroup in Player.TagGroupList )
		{
			var btn = TagButtons.Add.Button( tagGroup.Title, () => { SetTag( tagGroup ); } );
			btn.Style.FontColor = tagGroup.Color;
		}

		TagButtons.Enabled( false );
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
		if ( (_client.Pawn as Player).Status is not PlayerStatus.Alive || _client.Pawn.IsLocalPawn )
			return;

		if ( TagButtons.IsEnabled() )
		{
			Style.Set( "height", "38px" );
			TagButtons.Enabled( false );
		}
		else
		{
			Style.Set( "height", "76px" );
			TagButtons.Enabled( true );
		}
	}

	private void SetTag( ColorGroup tagGroup )
	{
		if ( tagGroup.Title == Tag.Text )
		{
			ResetTag();
			return;
		}

		var player = Local.Pawn as Player;

		Tag.Text = tagGroup.Title;
		Tag.Style.FontColor = tagGroup.Color;

		player.TaggedPlayers[_client.Pawn] = tagGroup;
	}

	private void ResetTag()
	{
		Tag.Text = string.Empty;
		var player = Local.Pawn as Player;
		try
		{
			player.TaggedPlayers.Remove( _client.Pawn );
		}
		catch ( NullReferenceException )
		{ }
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		ResetTag();
	}
}
