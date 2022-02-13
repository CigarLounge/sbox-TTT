using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

using TTT.Items;

namespace TTT.Player
{
	public partial class Perks : EntityComponent
	{
		[Net]
		private IList<Perk> _perks { get; set; }

		public int Count { get => _perks.Count; }

		public void Add( Perk perk )
		{
			if ( perk == null || perk is not IItem )
				return;

			_perks.Add( perk );
		}

		public bool Has( Type t )
		{
			return _perks.Any( x => x.GetType() == t );
		}

		public bool Contains( Perk perk )
		{
			return _perks.Contains( perk );
		}

		public Perk Get( int i )
		{
			return _perks[i];
		}

		public T Find<T>() where T : Perk
		{
			foreach ( var perk in _perks )
			{
				if ( perk is not T t || t.Equals( default( T ) ) )
					continue;

				return t;
			}
			return default;
		}

		public void Clear()
		{
			_perks.Clear();
		}
	}
}
