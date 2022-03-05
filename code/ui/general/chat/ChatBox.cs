
using Sandbox;
using Sandbox.UI;

[UseTemplate]
public partial class ChatBox : Panel
{

	public static ChatBox Current;

	public Panel EntryCanvas { get; set; }
	public TextEntry Input { get; set; }

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

	public ChatBox()
	{
		Current = this;

		Sandbox.Hooks.Chat.OnOpenChat += () =>
		{
			IsOpen = !IsOpen;
		};

		EntryCanvas.PreferScrollToBottom = true;
		EntryCanvas.TryScrollToBottom();

		Input.AddEventListener( "onsubmit", Submit );
		Input.AddEventListener( "onblur", () => IsOpen = false );
	}

	public override void OnHotloaded()
	{
		base.OnHotloaded();

		EntryCanvas.TryScrollToBottom();
	}

	public override void Tick()
	{
		base.Tick();

		if ( !IsOpen ) return;

		Input.Placeholder = string.Empty;
	}

	public void AddEntry( string name, string message, string c = default )
	{
		var entry = new ChatEntry( name, message );
		if ( !string.IsNullOrEmpty( c ) ) entry.AddClass( c );
		EntryCanvas.AddChild( entry );
	}

	private void Submit()
	{
		if ( string.IsNullOrWhiteSpace( Input.Text ) ) return;

		SendChat( Input.Text );
	}

	[ServerCmd]
	public static void SendChat( string message )
	{
		if ( !ConsoleSystem.Caller.IsValid() ) return;

		AddChat( To.Everyone, ConsoleSystem.Caller.Name, message );
	}

	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChat( string name, string message )
	{
		Current?.AddEntry( name, message );
	}

	[ClientCmd( "chat_add_info", CanBeCalledFromServer = true )]
	public static void AddInfo( string message, string name = "Server" )
	{
		Current?.AddEntry( name, message, "info" );
	}

	[ClientCmd( "chat_add_custom", CanBeCalledFromServer = true )]
	public static void AddCustom( string message, string c = default )
	{
		Current?.AddEntry( string.Empty, message, "custom " + c );
	}

}

