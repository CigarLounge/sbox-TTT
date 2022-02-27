using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class ChatBox : Panel
{
	public enum Channel { Info, Player, Spectator, Team };
	public static ChatBox Instance { get; private set; }

	public readonly List<ChatEntry> Messages = new();

	public const int MAX_MESSAGES_COUNT = 200;
	public const float MAX_DISPLAY_TIME = 8f;

	public bool IsOpened { get; private set; } = false;
	public bool IsTeamChatting { get; private set; } = false;

	private TimeSince _lastChatFocus = 0f;

	private readonly Panel _canvas;
	private readonly Panel _inputPanel;
	private readonly Panel _inputTeamIndicator;
	private readonly ChatBoxTextEntry _inputField;

	public ChatBox() : base()
	{
		Instance = this;

		StyleSheet.Load( "/ui/general/chat/ChatBox.scss" );

		_canvas = new Panel( this );
		_canvas.AddClass( "chat-canvas" );
		_canvas.AddClass( "rounded" );
		_canvas.PreferScrollToBottom = true;

		_inputPanel = new Panel( this );
		_inputPanel.AddClass( "input-panel" );
		_inputPanel.AddClass( "background-color-primary" );
		_inputPanel.AddClass( "opacity-zero" );
		_inputPanel.AddClass( "rounded" );

		_inputTeamIndicator = new Panel( _inputPanel );
		_inputTeamIndicator.AddClass( "input-team-indicator" );
		_inputTeamIndicator.AddClass( "circular" );

		_inputField = new( _inputPanel );
		_inputField.CaretColor = Color.White;
		_inputField.AcceptsFocus = true;
		_inputField.AllowEmojiReplace = true;
		_inputField.Text = "";
		_inputField.AddClass( "input-field" );
		_inputField.AddEventListener( "onsubmit", Submit );
		_inputField.AddEventListener( "onblur", Close );

		Sandbox.Hooks.Chat.OnOpenChat += Open;
	}

	public void OnTab()
	{
		if ( Local.Pawn is not Player player || player.LifeState != LifeState.Alive )
			return;

		if ( CanUseTeamChat( player ) )
			IsTeamChatting = !IsTeamChatting;
	}

	public override void Tick()
	{
		base.Tick();

		bool isAlive = Local.Pawn.LifeState == LifeState.Alive;

		if ( isAlive && Local.Pawn is Player player && IsTeamChatting )
		{
			_inputTeamIndicator.Style.BackgroundColor = player.Role.Info.Color;
			_inputPanel.Style.BorderColor = player.Role.Info.Color;
		}
		else
		{
			_inputTeamIndicator.Style.BackgroundColor = null;
			_inputPanel.Style.BorderColor = null;
		}

		_inputTeamIndicator.SetClass( "background-color-alive", isAlive );
		_inputTeamIndicator.SetClass( "background-color-spectator", !isAlive );
		_inputPanel.SetClass( "border-color-alive", isAlive );
		_inputPanel.SetClass( "border-color-spectator", !isAlive );

		if ( IsOpened )
			_lastChatFocus = 0f;

		_canvas.SetClass( "fadeOut", _lastChatFocus > MAX_DISPLAY_TIME );
	}

	private void Open()
	{
		IsOpened = true;

		_inputPanel.SetClass( "opacity-medium", true );
		_inputPanel.SetClass( "open", true );

		_inputField.Focus();
	}

	private void Close()
	{
		IsTeamChatting = false;
		IsOpened = false;

		_inputPanel.SetClass( "opacity-medium", false );
		_inputPanel.SetClass( "open", false );

		_inputField.Text = "";
		_inputField.Blur();
	}

	private void Submit()
	{
		bool wasTeamChatting = IsTeamChatting;

		string msg = _inputField.Text.Trim();

		if ( !string.IsNullOrWhiteSpace( msg ) && Local.Pawn is Player )
		{
			if ( wasTeamChatting )
			{
				SayTeam( msg );
			}
			else
			{
				Say( msg );
			}
		}

		Close();
	}

	public void AddEntry( string header, string content, Channel channel, string avatar = null, Team team = Team.None )
	{
		_lastChatFocus = 0f;

		if ( channel == Channel.Team )
		{
			Log.Error( "Cannot add chat entry to Team channel without a team name." );

			return;
		}

		header ??= "";

		#region Cleanup Old Messages
		if ( Messages.Count > MAX_MESSAGES_COUNT )
		{
			ChatEntry entry = Messages[0];
			Messages.RemoveAt( 0 );
			entry.Delete();
		}
		#endregion

		ChatEntry chatEntry = _canvas.AddChild<ChatEntry>();

		chatEntry.Header.Text = header;
		chatEntry.Content.Text = content;
		chatEntry.Channel = channel;

		chatEntry.Header.SetClass( "disable", string.IsNullOrEmpty( header ) );
		chatEntry.Header.SetClass( "header", chatEntry.Channel != Channel.Info );
		chatEntry.Content.SetClass( "disable", string.IsNullOrEmpty( content ) );
		chatEntry.Content.SetClass( "header", chatEntry.Channel == Channel.Info );
		chatEntry.Content.SetClass( "text-color-info", chatEntry.Channel == Channel.Info );

		switch ( channel )
		{
			case Channel.Info:
				chatEntry.Header.AddClass( "text-color-info" );

				break;

			case Channel.Player:
				chatEntry.Header.AddClass( "text-color-alive" );

				break;

			case Channel.Spectator:
				chatEntry.Header.AddClass( "text-color-spectator" );

				break;

			case Channel.Team:
				chatEntry.Header.Style.FontColor = team.GetColor();

				break;
		}

		bool showHeader =
			Messages.Count == 0 ||
			Messages[^1].Channel != chatEntry.Channel ||
			!Messages[^1].Header.Text.Equals( chatEntry.Header.Text ) ||
			chatEntry.Channel == Channel.Info;

		if ( showHeader )
		{
			chatEntry.Avatar.SetTexture( avatar );
		}

		chatEntry.SetClass( "show-header", showHeader );

		Messages.Add( chatEntry );
	}
	public static bool CanUseTeamChat( Player player )
	{
		return player.IsAlive() && player.Team == Team.Traitors;
	}

	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, Channel channel, string avatar = null, Team team = Team.None )
	{
		Instance?.AddEntry( name, message, channel, avatar, team );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		Instance?.AddEntry( null, message, Channel.Info, avatar );
	}

	[ServerCmd( "say" )]
	public static void Say( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// TODO: Consider RegEx to remove any messed up user chat messages.
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
		{
			return;
		}

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );

		LifeState lifeState = ConsoleSystem.Caller.Pawn.LifeState;

		if ( Game.Current?.Round is InProgressRound && lifeState == LifeState.Dead )
		{
			AddChatEntry( To.Multiple( Utils.GetClients( ( pl ) => pl.LifeState == LifeState.Dead ) ), ConsoleSystem.Caller.Name, message, Channel.Spectator, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
		}
		else
		{
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, Channel.Player, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
		}
	}

	[ServerCmd( "sayteam" )]
	public static void SayTeam( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// TODO: Consider RegEx to remove any messed up user chat messages.
		if ( ConsoleSystem.Caller.Pawn is not Player player || !CanUseTeamChat( player ) || message.Contains( '\n' ) || message.Contains( '\r' ) )
		{
			return;
		}

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );

		List<Client> clients = new();

		AddChatEntry( player.Team.ToClients(), ConsoleSystem.Caller.Name, message, Channel.Team, $"avatar:{ConsoleSystem.Caller.PlayerId}", player.Team );
	}
}

