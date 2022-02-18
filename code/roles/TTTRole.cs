using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Events;
using TTT.Globals;
using TTT.Player;
using TTT.Teams;

namespace TTT.Roles;

public abstract class TTTRole
{
	public virtual string Name => "None";
	public virtual Color Color => Color.Black;
	public virtual TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( NoneTeam ) );
	public virtual int DefaultCredits => 0;
	public static Dictionary<string, Shop> ShopDict { get; internal set; } = new();
	public virtual bool IsSelectable => true;

	public Shop Shop
	{
		get
		{
			ShopDict.TryGetValue( Name, out Shop shop );

			return shop;
		}
		internal set
		{
			ShopDict[Name] = value;
		}
	}

	public TTTRole() { }

	public virtual void OnSelect( TTTPlayer player )
	{
		player.Credits = Math.Max( DefaultCredits, player.Credits );

		if ( Host.IsServer )
		{
			player.Shop = Shop;
			player.ServerUpdateShop();
		}

		Event.Run( TTTEvent.Player.Role.Select, player );
	}

	public virtual void OnDeselect( TTTPlayer player ) { }

	public virtual void OnKilled( TTTPlayer killer ) { }

	// serverside function
	public virtual void InitShop()
	{
		Shop.Load( this );
	}

	public virtual void CreateDefaultShop() { }

	public virtual void UpdateDefaultShop( List<Type> newItemsList ) { }
}
