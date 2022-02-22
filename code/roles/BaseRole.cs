using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : Asset
{
	[Property] public Team Team { get; set; } = Team.None;
	[Property] public Color Color { get; set; }
	[Property] public int DefaultCredits { get; set; }
	[Property] public List<string> ExclusiveItems { get; set; } // It'd be cool if s&box let us select `Assets` here.

	public HashSet<string> AvailableItems { get; set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		AvailableItems = ExclusiveItems == null ? new HashSet<string>() : new HashSet<string>( ExclusiveItems );
	}
}

[Hammer.Skip]
public abstract class BaseRole : LibraryClass
{
	public RoleInfo Info { get; set; }

	public BaseRole()
	{
		Info = Asset.GetInfo<RoleInfo>( ClassInfo.Name );
	}

	public virtual void OnSelect( Player player )
	{
		if ( Host.IsServer )
		{
			player.Credits = Math.Max( Info.DefaultCredits, player.Credits );
			player.PurchasedLimitedShopItems.Clear();
		}

		Event.Run( TTTEvent.Player.Role.Selected, player );
	}

	public virtual void OnDeselect( Player player ) { }

	public virtual void OnKilled( Player player ) { }
}
