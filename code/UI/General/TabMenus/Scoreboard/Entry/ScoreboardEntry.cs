using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardEntry : Panel
{
	private static readonly ColorGroup[] _tagGroups = new ColorGroup[]
	{
		new ColorGroup("Friend", Color.FromBytes(0, 255, 0)),
		new ColorGroup("Suspect", Color.FromBytes(179, 179, 20)),
		new ColorGroup("Missing", Color.FromBytes(130, 190, 130)),
		new ColorGroup("Kill", Color.FromBytes(255, 0, 0))
	};

	private Image PlayerAvatar { get; init; }
	private Label PlayerName { get; init; }
	private Label Tag { get; init; }
	private Label Karma { get; init; }
	private Label Score { get; init; }
	private Label Ping { get; init; }
	private Panel DropdownPanel { get; set; }

	public PlayerStatus PlayerStatus;
	private readonly Client _client;

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

		SetClass( "me", _client == Local.Client );

		if ( player.Role is not NoneRole and not Innocent )
			Style.BackgroundColor = player.Role.Color.WithAlpha( 0.15f );
		else
			Style.BackgroundColor = null;

		PlayerAvatar.SetTexture( $"avatar:{_client.PlayerId}" );
	}

	public void OnClick()
	{
		if ( DropdownPanel is null )
		{
			DropdownPanel = new Panel( this, "dropdown" );
			foreach ( var tagGroup in _tagGroups )
			{
				var tagButton = DropdownPanel.Add.Button( tagGroup.Title, () => { SetTag( tagGroup ); } );
				tagButton.Style.FontColor = tagGroup.Color;
			}
		}
		else
		{
			DropdownPanel.Delete();
			DropdownPanel = null;
		}
	}

	private void SetTag( ColorGroup tagGroup )
	{
		if ( tagGroup.Title == Tag.Text )
		{
			ResetTag();
			return;
		}

		Tag.Text = tagGroup.Title;
		Tag.Style.FontColor = tagGroup.Color;
		(_client.Pawn as Player).TagGroup = tagGroup;
	}

	private void ResetTag()
	{
		Tag.Text = string.Empty;

		if ( _client.IsValid() && _client.Pawn.IsValid() )
			(_client.Pawn as Player).TagGroup = default;
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		ResetTag();
	}
}
