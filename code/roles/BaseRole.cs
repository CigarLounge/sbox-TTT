using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTT.Events;
using TTT.Globals;
using TTT.Player;

namespace TTT.Roles;

public abstract class BaseRole : BaseNetworkable
{
	public virtual Team Team => Team.None;
	public virtual string Name => "None";
	public virtual Color Color => Color.Black;
	public virtual int DefaultCredits => 0;

	public BaseRole() { }

	public virtual void OnSelect( TTTPlayer player )
	{
		player.Credits = Math.Max( DefaultCredits, player.Credits );

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
