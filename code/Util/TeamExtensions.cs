using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public static class TeamExtensions
{
	private static readonly Dictionary<Team, HashSet<IClient>> _clients = new();
	private static readonly Dictionary<Team, UI.ColorGroup> _properties = new();
	private static readonly Dictionary<Team, List<ItemInfo>> _shopItems = new();	

	static TeamExtensions()
	{
		// Default teams.
		Team.None.Initialize( "Nones", Color.Transparent );
		Team.Innocents.Initialize( "Innocents", Color.FromBytes( 26, 196, 77 ) );
		Team.Traitors.Initialize( "Traitors", Color.FromBytes( 223, 40, 52 ) );
	}

	public static void Initialize(this Team team, string title, Color color )
	{
		_properties[team] = new UI.ColorGroup
		{
			Title = title,
			Color = color
		};

		_clients.Add( team, new HashSet<IClient>() );
		_shopItems.Add( team, new List<ItemInfo>() );
	}

	public static string GetTitle( this Team team )
	{
		return _properties[team].Title;
	}

	public static Color GetColor( this Team team )
	{
		return _properties[team].Color;
	}

	public static List<ItemInfo> GetShopItems( this Team team )
	{
		return _shopItems.TryGetValue(team, out var list) ? list : null;
	}

	public static int GetCount( this Team team )
	{
		return _clients[team].Count;
	}

	public static To ToClients( this Team team )
	{
		return To.Multiple( _clients[team] );
	}

	public static To ToAliveClients( this Team team )
	{
		return To.Multiple( _clients[team].Where( x => x.Pawn is Player player && player.IsAlive ) );
	}

	[TTTEvent.Player.RoleChanged]
	private static void OnPlayerRoleChanged( Player player, Role oldRole )
	{
		if ( oldRole is not null )
			_clients[oldRole.Team].Remove( player.Client );

		if ( player.Role is not null )
			_clients[player.Team].Add( player.Client );
	}
}
