using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TextChat : Panel
{
	public static TextChat Instance;

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

	private static readonly Color _allChatColor = PlayerStatus.Alive.GetColor();
	private static readonly Color _spectatorChatColor = PlayerStatus.Spectator.GetColor();

	private const int MaxItems = 100;
	private const float MessageLifetime = 10f;

	private Panel Canvas { get; set; }
	private TextEntry Input { get; set; }

	private readonly Queue<TextChatEntry> _entries = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		Canvas.PreferScrollToBottom = true;
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;
		Input.OnTabPressed += OnTabPressed;

		Instance = this;
	}

	public override void Tick()
	{
		if ( Game.LocalPawn is not Player player )
			return;

		if ( Sandbox.Input.Pressed( InputButton.Chat ) )
			Open();

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
	}

	private void AddEntry( TextChatEntry entry )
	{
		Canvas.AddChild( entry );
		Canvas.TryScrollToBottom();

		entry.BindClass( "stale", () => entry.Lifetime > MessageLifetime );

		_entries.Enqueue( entry );
		if ( _entries.Count > MaxItems )
			_entries.Dequeue().Delete();
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
		var message = Input.Text.Trim();
		Input.Text = "";

		Close();

		if ( string.IsNullOrWhiteSpace( message ) )
			return;

		if ( message == "!rtv" && Game.LocalClient.HasRockedTheVote() )
		{
			AddInfoEntry( "You have already rocked the vote!" );
			return;
		}

		SendChat( message );
	}

	[ConCmd.Server( "ttt_say" )]
	public static void SendChat( string message )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( message == "!rtv" )
		{
			GameManager.RockTheVote();
			return;
		}

		if ( !player.IsAlive )
		{
			var clients = GameManager.Current.State is InProgress ? Utils.GetClientsWhere( p => !p.IsAlive ) : Game.Clients;
			AddChatEntry( To.Multiple( clients ), player.SteamId, player.SteamName, message, Channel.Spectator );
			return;
		}

		if ( player.CurrentChannel == Channel.All )
		{
			player.LastWords = message;
			AddChatEntry( To.Everyone, player.SteamId, player.SteamName, message, player.CurrentChannel, player.IsRoleKnown ? player.Role.Info.ResourceId : -1 );
		}
		else if ( player.CurrentChannel == Channel.Team && player.Role.CanTeamChat )
		{
			AddChatEntry( player.Team.ToClients(), player.SteamId, player.SteamName, message, player.CurrentChannel, player.Role.Info.ResourceId );
		}
	}

	[ClientRpc]
	public static void AddChatEntry( long playerId, string playerName, string message, Channel channel, int roleId = -1 )
	{
		switch ( channel )
		{
			case Channel.All:
				Instance?.AddEntry( new TextChatEntry( playerId, playerName, message, ResourceLibrary.Get<RoleInfo>( roleId )?.Color ?? _allChatColor ) );
				return;
			case Channel.Team:
				Instance?.AddEntry( new TextChatEntry( playerId, $"(TEAM) {playerName}", message, ResourceLibrary.Get<RoleInfo>( roleId ).Color ) );
				return;
			case Channel.Spectator:
				Instance?.AddEntry( new TextChatEntry( playerId, playerName, message, _spectatorChatColor ) );
				return;
		}
	}

	[ClientRpc]
	public static void AddInfoEntry( string message )
	{
		Instance?.AddEntry( new TextChatEntry( message, Color.FromBytes( 253, 196, 24 ) ) );
	}

	private void OnTabPressed()
	{
		if ( Game.LocalPawn is not Player player || !player.IsAlive )
			return;

		if ( player.Role.CanTeamChat )
			player.CurrentChannel = player.CurrentChannel == Channel.All ? Channel.Team : Channel.All;
	}
}

public partial class TextEntry : Sandbox.UI.TextEntry
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
