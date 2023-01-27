using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TextChat : Panel
{
	public static TextChat Current;

	public bool IsOpen
	{
		get => HasClass( "open" );
		set
		{
			SetClass( "open", value );
			if ( value )
			{
				Input.Focus();
				Input.Text = string.Empty;
				Input.Label.SetCaretPosition( 0 );
			}
		}
	}

	private const int MaxItems = 100;
	private const float MessageLifetime = 10f;

	private Panel Canvas { get; set; }
	private TextEntry Input { get; set; }

	private readonly Queue<ChatRow> _rows = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		Canvas.PreferScrollToBottom = true;
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;

		Current = this;
	}

	public override void Tick()
	{
		if ( Sandbox.Input.Pressed( InputButton.Chat ) )
			Open();
	}

	public void AddEntry( string name, string message, long playerId = 0, bool isInfo = false )
	{
		var entry = Canvas.AddChild<ChatRow>();

		var player = Game.LocalPawn;
		if ( !player.IsValid() )
			return;

		entry.Message = message;
		entry.Name = $"{name}";
		entry.PlayerId = playerId;

		entry.SetClass( "noname", string.IsNullOrEmpty( name ) );
		entry.SetClass( "info", isInfo );
		entry.BindClass( "stale", () => entry.Lifetime > MessageLifetime );

		Canvas.TryScrollToBottom();

		_rows.Enqueue( entry );

		if ( _rows.Count > MaxItems )
			_rows.Dequeue().Delete();
	}

	private void Open()
	{
		AddClass( "open" );
		Input.Focus();
		Canvas.TryScrollToBottom();
	}

	private void Close()
	{
		RemoveClass( "open" );
		Input.Blur();
		Input.Text = string.Empty;
		Input.Label.SetCaretPosition( 0 );
	}

	private void Submit()
	{
		var msg = Input.Text.Trim();
		Input.Text = "";

		Close();

		if ( string.IsNullOrWhiteSpace( msg ) ) return;

		Say( msg );
	}

	[ConCmd.Client( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string playerId = "0", bool isInfo = false )
	{
		Current?.AddEntry( name, message, long.Parse( playerId ), isInfo );

		// Only log clientside if we're not the listen server host
		if ( !Game.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	public static void AddChatEntry( To target, string name, string message, long playerId = 0, bool isInfo = false )
	{
		// Can't use long on ConCmd :<
		AddChatEntry( target, name, message, playerId.ToString(), isInfo );
	}

	[ConCmd.Server( "say" )]
	public static void Say( string message )
	{
		if ( !ConsoleSystem.Caller.IsValid() ) return;

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );
		AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, ConsoleSystem.Caller.SteamId );
	}
}
