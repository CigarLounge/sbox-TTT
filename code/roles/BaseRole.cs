using System;
using System.Collections.Generic;
using Sandbox;

namespace TTT;

[Library( "role" ), AutoGenerate]
public partial class RoleInfo : Asset
{
	[Property] public Team Team { get; set; } = Team.None;
	[Property] public Color Color { get; set; }
	/// <summary>
	/// The amount of credits player's spawn with.
	/// </summary>
	[Property] public int DefaultCredits { get; set; }
	[Property] public List<string> ExclusiveItems { get; set; } // It'd be cool if s&box let us select `Assets` here.
	/// <summary>
	/// Players can retrieve credits from corpses.
	/// </summary>
	[Property] public bool RetrieveCredits { get; set; }
	/// <summary>
	/// Player's role will always be revealed to other players.
	/// </summary>
	[Property] public bool RevealToEveryone { get; set; }

	public HashSet<string> AvailableItems { get; set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		AvailableItems = ExclusiveItems == null ? new HashSet<string>() : new HashSet<string>( ExclusiveItems );
	}
}

[Hammer.Skip]
public abstract class BaseRole : LibraryClass, IEquatable<BaseRole>, IEquatable<string>
{
	public RoleInfo Info { get; set; }

	public BaseRole()
	{
		Info = Asset.GetInfo<RoleInfo>( this );
	}

	public virtual void OnSelect( Player player )
	{
		if ( Host.IsServer )
		{
			player.Credits = Math.Max( Info.DefaultCredits, player.Credits );
			player.PurchasedLimitedShopItems.Clear();

			if ( Info.RevealToEveryone )
			{
				player.IsConfirmed = true;

				foreach ( var otherPlayer in Utils.GetPlayers( x => x != player ) )
				{
					player.SendClientRole( To.Single( otherPlayer ) );
				}
			}
		}
	}

	public virtual void OnDeselect( Player player ) { }

	public virtual void OnKilled( Player player ) { }

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
		if ( Info.LibraryName.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		if ( Info.Title.Equals( other, StringComparison.OrdinalIgnoreCase ) )
			return true;

		return false;
	}

	public override bool Equals( object obj ) => Equals( obj as BaseRole );
	public override int GetHashCode() => Info.Id.GetHashCode();
}
