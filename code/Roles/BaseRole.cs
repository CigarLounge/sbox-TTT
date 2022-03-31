using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TTT;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : Asset
{
	[Property]
	public Team Team { get; set; } = Team.None;

	[Property( "defaultcredits", "The amount of credits players spawn with." )]
	public int DefaultCredits { get; set; } = 0;

	[Property]
	public List<string> ExclusiveItems { get; set; } // It'd be cool if s&box let us select `Assets` here.

	[Property( "retrievecredits", "Players can retrieve credits from corpses." )]
	public bool RetrieveCredits { get; set; } = false;

	[Property]
	public bool CanRoleChat { get; set; } = false;

	[Property, Category( "UI" )]
	public Color Color { get; set; }

	[Property, Category( "UI" ), ResourceType( "png" )]
	public string Icon { get; set; } = "ui/logo.png";

	public HashSet<string> AvailableItems { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		AvailableItems = new HashSet<string>( ExclusiveItems );
	}
}

public abstract class BaseRole : LibraryClass, IEquatable<BaseRole>, IEquatable<string>
{
	public RoleInfo Info { get; private set; }

	public Team Team => Info.Team;
	public Color Color => Info.Color;
	public HashSet<string> AvailableItems => Info.AvailableItems;
	public bool RetrieveCredits => Info.RetrieveCredits;
	public bool CanRoleChat => Info.CanRoleChat;
	public string Title => Info.Title;

	public BaseRole()
	{
		Info = Asset.GetInfo<RoleInfo>( this );
	}

	public virtual void OnSelect( Player player )
	{
		if ( player.IsLocalPawn )
		{
			if ( Info.AvailableItems.Count > 0 )
				UI.RoleMenu.Instance.AddShopTab();

			Player.RoleButtons = GetRoleButtons();
			foreach ( var roleButton in Player.RoleButtons )
				Player.RoleButtonPoints.Add( new UI.RoleButtonPoint( roleButton ) );

			return;
		}

		player.Credits = Math.Max( Info.DefaultCredits, player.Credits );
		player.PurchasedLimitedShopItems.Clear();
	}

	public virtual void OnDeselect( Player player )
	{
		if ( player.IsLocalPawn )
		{
			player.ClearButtons();

			if ( Info.AvailableItems.Count > 0 )
				UI.RoleMenu.Instance.RemoveTab( RawStrings.ShopTab );
		}
	}

	public virtual void OnKilled( Player player ) { }

	protected List<RoleButton> GetRoleButtons()
	{
		Host.AssertClient();

		return Entity.All
				.OfType<RoleButton>()
				.Where( x => x.Role == "All" || this == x.Role )
				.ToList();
	}

	public static bool operator ==( BaseRole left, BaseRole right )
	{
		if ( left is null )
		{
			if ( right is null )
				return true;

			return false;
		}

		return left.Equals( right );
	}
	public static bool operator !=( BaseRole left, BaseRole right ) => !(left == right);

	public static bool operator ==( BaseRole left, string right )
	{
		if ( left is null || right is null )
			return false;

		return left.Equals( right );
	}
	public static bool operator !=( BaseRole left, string right ) => !(left == right);

	public bool Equals( BaseRole other )
	{
		if ( other is null )
			return false;

		if ( Object.ReferenceEquals( this, other ) )
			return true;

		return Info.Id == other.Info.Id;
	}

	public bool Equals( string other )
	{
		if ( Info.Title.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		if ( Info.LibraryName.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		return false;
	}

	public override bool Equals( object obj ) => Equals( obj as BaseRole );

	public override int GetHashCode() => Info.Id.GetHashCode();

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = Asset.GetInfo<RoleInfo>( this );
		Player.RoleButtons = GetRoleButtons();
	}
#endif
}
