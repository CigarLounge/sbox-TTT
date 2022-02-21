using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : Asset
{
	[Property] public Team Team { get; set; } = Team.None;
	[Property] public Color Color { get; set; } // https://github.com/Facepunch/sbox-issues/issues/928
	[Property] public int DefaultCredits { get; set; }
	[Property] public List<string> ExclusiveItems { get; set; } // It'd be cool if s&box let us select `Assets` here.

	public List<string> AvailableItems { get; set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		// Blame s&box
		// https://github.com/Facepunch/sbox-issues/issues/928
		if ( LibraryName == "ttt_role_none" )
			Color = Color.Transparent;
		else if ( LibraryName == "ttt_role_traitor" )
			Color = Color.FromBytes( 223, 41, 53 );
		else if ( LibraryName == "ttt_role_innocent" )
			Color = Color.FromBytes( 27, 197, 78 );
		else if ( LibraryName == "ttt_role_detective" )
			Color = Color.FromBytes( 25, 102, 255 );

		AvailableItems = ExclusiveItems?.Distinct().ToList() ?? new List<string>();
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
			player.PurchasedShopItems.Clear();
		}

		Event.Run( TTTEvent.Player.Role.Selected, player );
	}

	public virtual void OnDeselect( Player player ) { }

	public virtual void OnKilled( Player killer ) { }
}
