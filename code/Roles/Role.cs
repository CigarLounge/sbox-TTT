using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public abstract class Role : IEquatable<string>
{
	public static Innocent Innocent { get; private set; }
	public static Traitor Traitor { get; private set; }
	public static Detective Detective { get; private set; }
	public static NoneRole None { get; private set; }

	public RoleInfo Info { get; private set; }
	public HashSet<Player> Players { get; private init; } = new();
	public Team Team => Info.Team;
	public Color Color => Info.Color;
	public HashSet<ItemInfo> ShopItems => Info.ShopItems;
	public bool CanRetrieveCredits => Info.CanRetrieveCredits;
	public bool CanTeamChat => Info.CanTeamChat;
	public bool CanAttachCorpses => Info.CanAttachCorpses;
	public string Title => Info.Title;
	public RoleInfo.KarmaConfig Karma => Info.Karma;
	public RoleInfo.ScoringConfig Scoring => Info.Scoring;

	protected Role()
	{
		Info = GameResource.GetInfo<RoleInfo>( GetType() );
	}

	public virtual void OnSelect( Player player )
	{
		if ( player.IsLocalPawn )
		{
			if ( Info.ShopItems.Count > 0 )
				UI.RoleMenu.Instance.AddShopTab();

			Player.RoleButtons = GetRoleButtons();

			foreach ( var roleButton in Player.RoleButtons )
				Player.RoleButtonPoints.Add( new UI.RoleButtonPoint( roleButton ) );

			return;
		}
		else if ( !Host.IsServer )
		{
			if ( ShouldCreateRolePlate( player ) )
				player.Components.Create<UI.RolePlate>();

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

			if ( Info.ShopItems.Any() )
				UI.RoleMenu.Instance.RemoveTab( UI.RoleMenu.ShopTab );
		}
		else if ( !Host.IsServer )
		{
			player.Components.RemoveAny<UI.RolePlate>();
		}
	}

	protected virtual bool ShouldCreateRolePlate( Player player )
	{
		return false;
	}

	protected List<RoleButton> GetRoleButtons()
	{
		Host.AssertClient();

		return Entity.All
				.OfType<RoleButton>()
				.Where( x => x.IsValid() && (x.RoleName == "All" || this == x.RoleName) )
				.ToList();
	}

	public static void Init()
	{
		Innocent = new Innocent();
		Traitor = new Traitor();
		Detective = new Detective();
		None = new NoneRole();
	}

	[GameEvent.Player.RoleChanged]
	private static void OnPlayerRoleChanged( Player player, Role oldRole )
	{
		oldRole?.Players.Remove( player );
		player.Role?.Players.Add( player );
	}

	public static bool operator ==( Role left, string right )
	{
		if ( left is null || right is null )
			return false;

		return left.Equals( right );
	}
	public static bool operator !=( Role left, string right ) => !(left == right);

	public bool Equals( Role other )
	{
		if ( other is null )
			return false;

		if ( ReferenceEquals( this, other ) )
			return true;

		return Info == other.Info;
	}

	public bool Equals( string other )
	{
		if ( Info.Title.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		if ( Info.ClassName.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		return false;
	}

	public override bool Equals( object obj )
	{
		if ( ReferenceEquals( this, obj ) )
			return true;

		if ( obj is null )
			return false;

		throw new NotImplementedException();
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotload()
	{
		Info = GameResource.GetInfo<RoleInfo>( GetType() );
		Player.RoleButtons = GetRoleButtons();
	}
#endif
}
