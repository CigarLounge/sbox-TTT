using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class Perks : EntityComponent
{
	public Player Owner { get; set; }
	private List<Perk> _perks { get; set; } = new();
	public int Count => _perks.Count;

	public Perks( Player player )
	{
		Owner = player;
	}

	public void Add( Perk perk )
	{
		_perks.Add( perk );
		Owner.Components.Add( perk );
	}

	public void Remove( Perk perk )
	{
		_perks.Remove( perk );
		Owner.Components.Remove( perk );
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
		foreach ( var perk in _perks.ToArray() )
		{
			Remove( perk );
		}
	}
}
