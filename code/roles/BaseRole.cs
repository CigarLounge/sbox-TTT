using System;
using System.Collections.Generic;

using Sandbox;

namespace TTT;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : Asset
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
		Info = Asset.GetInfo<RoleInfo>( ClassInfo.Name );
	}

	public virtual void OnSelect( Player player )
	{
		player.Credits = Math.Max( Info.DefaultCredits, player.Credits );

		if ( Host.IsServer )
			player.ServerUpdateShop();

		Event.Run( TTTEvent.Player.Role.Selected, player );
	}

	public virtual void OnDeselect( Player player ) { }

	public virtual void OnKilled( Player killer ) { }

	// serverside function
	public virtual void InitShop()
	{
		// Shop.Load( this );
	}

	public virtual void CreateDefaultShop() { }

	public virtual void UpdateDefaultShop( List<Type> newItemsList ) { }
}
