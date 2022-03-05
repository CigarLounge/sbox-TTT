
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class ChatBox : Panel
{
	public enum Channel
	{
		Alive,
		Spectator,
		Role // For detectives & traitors
	}

	public static ChatBox Instance;

	public Panel EntryCanvas { get; set; }
	public TabTextEntry Input { get; set; }
	public Channel CurrentChannel { get; private set; } = Channel.Alive;

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
		Instance = this;

		Sandbox.Hooks.Chat.OnOpenChat += () =>
		{
			IsOpen = !IsOpen;
		};

		EntryCanvas.PreferScrollToBottom = true;
		EntryCanvas.TryScrollToBottom();

		Input.AddEventListener( "onsubmit", Submit );
		Input.AddEventListener( "onblur", () => IsOpen = false );
		Input.OnTabPressed += OnTabPressed;
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

		SendChat( Input.Text, CurrentChannel );
	}

	[ServerCmd]
	public static void SendChat( string message, Channel channel )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player )
			return;

		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		switch ( channel )
		{
			case Channel.Alive:
				AddChat( To.Everyone, ConsoleSystem.Caller.Name, message );
				break;
			case Channel.Role:
				AddChat( To.Everyone, ConsoleSystem.Caller.Name, message );
				break;
			case Channel.Spectator:
				AddChat( To.Everyone, ConsoleSystem.Caller.Name, message );
				break;
		}
	}

	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChat( string name, string message )
	{
		Instance?.AddEntry( name, message );
	}

	[ClientCmd( "chat_add_info", CanBeCalledFromServer = true )]
	public static void AddInfo( string message )
	{
		Instance?.AddEntry( message, "", "info" );
	}

	private void OnTabPressed()
	{
		if ( Local.Pawn.IsAlive() && CanTeamChat() )
		{
			if ( CurrentChannel == Channel.Alive )
				CurrentChannel = Channel.Role;
			else if ( CurrentChannel == Channel.Role )
				CurrentChannel = Channel.Alive;
		}
	}

	private static bool CanTeamChat()
	{
		return Local.Pawn is Player player && (player.Role is DetectiveRole || player.Role is TraitorRole);
	}
}

