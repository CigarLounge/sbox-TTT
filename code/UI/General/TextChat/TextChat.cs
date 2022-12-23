using System;
using System.Text.Json;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class TextChat : Panel
{
	private static readonly Color _allChatColor = PlayerStatus.Alive.GetColor();
	private static readonly Color _spectatorChatColor = PlayerStatus.Spectator.GetColor();

	public static TextChat Instance { get; private set; }

	private Panel EntryCanvas { get; set; }
	private TabTextEntry Input { get; set; }

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

	public TextChat() => Instance = this;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		Input.AddEventListener( "onsubmit", Submit );
		Input.AddEventListener( "onblur", () => IsOpen = false );
		Input.OnTabPressed += OnTabPressed;
		EntryCanvas.TryScrollToBottom();
		EntryCanvas.PreferScrollToBottom = true;
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

	public void AddChatEntry( string name, string message, Color color )
	{
		var entry = new TextChatEntry() { Name = name, Message = message, NameColor = color };
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
				DisplayInfoMessage( "You have already rocked the vote!" );
				return;
			}
		}

		SendChatMessage( Input.Text );
	}

	[ConCmd.Server]
	public static void SendChatMessage( string message )
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
			DisplayChatMessage( To.Multiple( clients ), player.Client.Name, message, Channel.Spectator );
			return;
		}

		if ( player.CurrentChannel == Channel.All )
		{
			player.LastWords = message;
			DisplayChatMessage( To.Everyone, player.Client.Name, message, player.CurrentChannel, player.IsRoleKnown ? player.Role.Info.ResourceId : -1 );
		}
		else if ( player.CurrentChannel == Channel.Team && player.Role.CanTeamChat )
		{
			DisplayChatMessage( player.Team.ToClients(), player.Client.Name, message, player.CurrentChannel, player.Role.Info.ResourceId );
		}
	}

	[ClientRpc]
	public static void DisplayChatMessage( string name, string message, Channel channel, int roleId = -1 )
	{
		switch ( channel )
		{
			case Channel.All:
				Instance?.AddChatEntry( name, message, roleId != -1 ? ResourceLibrary.Get<RoleInfo>( roleId ).Color : _allChatColor );
				return;
			case Channel.Team:
				Instance?.AddChatEntry( $"(TEAM) {name}", message, ResourceLibrary.Get<RoleInfo>( roleId ).Color );
				return;
			case Channel.Spectator:
				Instance?.AddChatEntry( name, message, _spectatorChatColor );
				return;
		}
	}

	[ClientRpc]
	public static void DisplayInfoMessage( string message )
	{
		Instance?.AddChatEntry( message, string.Empty, Color.FromBytes( 253, 196, 24 ) );
	}

	[ConCmd.Server]
	public static void SendQuickChat( string prefix, string suffix, string message, string colorJson )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player || !player.IsAlive() )
			return;

		DisplayQuickChatMessage(
			To.Everyone,
			player.SteamName,
			player.IsRoleKnown ? player.Role.Info.ResourceId : -1,
			prefix,
			suffix,
			message,
			JsonSerializer.Deserialize<Color>( colorJson )
		);
	}

	[ClientRpc]
	public static void DisplayQuickChatMessage( string name, int roleId, string prefix, string suffix, string message, Color messageColor )
	{
		var entry = new TextChatEntry() { Name = name, NameColor = roleId != -1 ? ResourceLibrary.Get<RoleInfo>( roleId ).Color : _allChatColor };
		entry.Add.Label( prefix );
		entry.Add.Label( message ).Style.FontColor = messageColor;
		entry.Add.Label( suffix );
		Instance.EntryCanvas.AddChild( entry );
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
