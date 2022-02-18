using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Events;
using TTT.Player;

namespace TTT.Roles;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : AssetInfo
{
	[Property] public Team Team { get; set; } = Team.None;
	[Property] public Color Color { get; set; }
	[Property] public int DefaultCredits { get; set; }
}

[Hammer.Skip]
public abstract class BaseRole : LibraryClass
{
	public RoleInfo Info { get; set; }

	public BaseRole()
	{
		Info = AssetInfo.Collection[ClassInfo?.Name] as RoleInfo;
	}

	public virtual void OnSelect( TTTPlayer player )
	{
		player.Credits = Math.Max( Info.DefaultCredits, player.Credits );

		if ( Host.IsServer )
		{
			player.ServerUpdateShop();
		}

		Event.Run( TTTEvent.Player.Role.Selected, player );
	}

	public virtual void OnDeselect( TTTPlayer player ) { }

	public virtual void OnKilled( TTTPlayer killer ) { }

	// serverside function
	public virtual void InitShop()
	{
		// Shop.Load( this );
	}

	public virtual void CreateDefaultShop() { }

	public virtual void UpdateDefaultShop( List<Type> newItemsList ) { }
}
