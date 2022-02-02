using System.Collections.Generic;

using Sandbox;

using TTT.Items;

namespace TTT.Player
{
	public partial class PerksInventory
	{
		private List<TTTPerk> PerkList { get; } = new();
		private readonly TTTPlayer _owner;

		public PerksInventory( TTTPlayer owner )
		{
			_owner = owner;
		}

		public bool Give( TTTPerk perk )
		{
			PerkList.Add( perk );
			perk.Equip( _owner );
			return true;
		}

		public bool Take( TTTPerk perk )
		{
			PerkList.Remove( perk );
			perk.Remove();
			perk.Delete();
			return true;
		}

		public T Find<T>( string perkName = null ) where T : TTTPerk
		{
			foreach ( TTTPerk loopPerk in PerkList )
			{
				if ( loopPerk is not T t || t.Equals( default( T ) ) )
				{
					continue;
				}

				if ( perkName == null )
				{
					return t;
				}
			}

			return default;
		}

		public TTTPerk Find( string perkName )
		{
			return Find<TTTPerk>( perkName );
		}

		public bool Has( string perkName = null )
		{
			return Find( perkName ) != null;
		}

		public bool Has<T>( string perkName = null ) where T : TTTPerk
		{
			return Find<T>( perkName ) != null;
		}

		public void Clear()
		{
			foreach ( TTTPerk perk in PerkList )
			{
				perk.Remove();
				perk.Delete();
			}
			PerkList.Clear();
		}

		public int Count()
		{
			return PerkList.Count;
		}

		public TTTPerk Get( int index )
		{
			return PerkList[index];
		}
	}
}
