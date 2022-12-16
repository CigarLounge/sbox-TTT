using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class TextChat : Panel
{
	private static readonly Color _allChatColor = PlayerStatus.Alive.GetColor();
	private static readonly Color _spectatorChatColor = PlayerStatus.Spectator.GetColor();

	public static TextChat Instance { get; private set; }

	private Panel EntryCanvas { get; init; }
	private TabTextEntry Input { get; init; }

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

	public TextChat()
	{
		Instance = this;

		EntryCanvas.PreferScrollToBottom = true;
		EntryCanvas.TryScrollToBottom();

		Input.AddEventListener( "onsubmit", Submit );
		Input.AddEventListener( "onblur", () => IsOpen = false );
		Input.OnTabPressed += OnTabPressed;
	}

	public override void Tick()
	{
		if ( Game.LocalPawn is not Player player )
			return;

		if ( Sandbox.Input.Pressed( InputButton.Chat ) )
			IsOpen = true;

		if ( !IsOpen )
			return;

		switch ( player.CurrentChannel )
		{
			case Channel.All:
				Input.Style.BorderColor = _allChatColor;
				return;
			case Channel.Spectator:
				Input.Style.BorderColor = _spectatorChatColor;
				return;
			case Channel.Team:
				Input.Style.BorderColor = player.Role.Color;
				return;
		}

		Input.Placeholder = string.Empty;
	}

	public void AddEntry( string name, string message, Color? color = null )
	{
		var entry = new TextChatEntry() { Name = name, Message = message, NameColor = color ?? Color.FromBytes( 253, 196, 24 ) };
		EntryCanvas.AddChild( entry );
	}

	private void Submit()
	{
		if ( Input.Text.IsNullOrEmpty() )
			return;

		if ( Input.Text.Contains( '\n' ) || Input.Text.Contains( '\r' ) )
			return;

		if ( Input.Text == Strings.RTVCommand )
		{
			if ( Game.LocalClient.GetValue<bool>( Strings.HasRockedTheVote ) )
			{
				AddInfo( "You have already rocked the vote!" );
				return;
			}
		}

		SendChat( Input.Text );
	}

	[ConCmd.Server]
	public static void SendChat( string message )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( message == Strings.RTVCommand )
		{
			GameManager.RockTheVote();
			return;
		}

		if ( !player.IsAlive() )
		{
			var clients = GameManager.Current.State is InProgress ? Utils.GetDeadClients() : Game.Clients;
			AddChat( To.Multiple( clients ), player.Client.Name, message, Channel.Spectator );
			return;
		}

		if ( player.CurrentChannel == Channel.All )
		{
			player.LastWords = message;
			AddChat( To.Everyone, player.Client.Name, message, player.CurrentChannel, player.IsRoleKnown ? player.Role.Info.ResourceId : -1 );
		}
		else if ( player.CurrentChannel == Channel.Team && player.Role.CanTeamChat )
		{
			AddChat( player.Team.ToClients(), player.Client.Name, message, player.CurrentChannel, player.Role.Info.ResourceId );
		}
	}

	[ConCmd.Client( "ttt_chat_add", CanBeCalledFromServer = true )]
	public static void AddChat( string name, string message, Channel channel, int roleId = -1 )
	{
		switch ( channel )
		{
			case Channel.All:
				Instance?.AddEntry( name, message, roleId != -1 ? ResourceLibrary.Get<RoleInfo>( roleId ).Color : _allChatColor );
				return;
			case Channel.Team:
				Instance?.AddEntry( $"(TEAM) {name}", message, ResourceLibrary.Get<RoleInfo>( roleId ).Color );
				return;
			case Channel.Spectator:
				Instance?.AddEntry( name, message, _spectatorChatColor );
				return;
		}
	}

	[ConCmd.Client( "ttt_chat_add_info", CanBeCalledFromServer = true )]
	public static void AddInfo( string message )
	{
		Instance?.AddEntry( message, "" );
	}

	private void OnTabPressed()
	{
		if ( Game.LocalPawn is not Player player || !player.IsAlive() )
			return;

		if ( player.Role.CanTeamChat )
			player.CurrentChannel = player.CurrentChannel == Channel.All ? Channel.Team : Channel.All;
	}
}

public partial class TabTextEntry : TextEntry
{
	public event Action OnTabPressed;

	public override void OnButtonTyped( string button, KeyModifiers km )
	{
		if ( button == "tab" )
		{
			OnTabPressed?.Invoke();
			return;
		}

		base.OnButtonTyped( button, km );
	}
}
