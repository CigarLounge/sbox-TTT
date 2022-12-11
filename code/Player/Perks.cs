using Sandbox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

/// <summary>
/// A sublist of <see cref="Entity.Components"/> that contains components
/// of type <see cref="TTT.Perk"/>.
/// </summary>
public class Perks : IEnumerable<Perk>
{
	public Player Owner { get; private init; }

	public Perk this[int i] => _list[i];

	private readonly List<Perk> _list = new();
	public int Count => _list.Count;

	public Perks( Player player ) => Owner = player;

	public void Add( Perk perk )
	{
		Game.AssertServer();

		Owner.Components.Add( perk );
	}

	public void Remove( Perk perk )
	{
		Game.AssertServer();

		Owner.Components.Remove( perk );
	}

	public bool Has<T>()
	{
		return _list.Any( x => x is T );
	}

	public bool Contains( Perk perk ) => _list.Contains( perk );

	public T Find<T>() where T : Perk
	{
		foreach ( var perk in _list )
		{
			if ( perk is not T t || t.Equals( default( T ) ) )
				continue;

			return t;
		}
		return default;
	}

	public void DeleteContents()
	{
		foreach ( var perk in _list.ToArray() )
			Owner.Components.Remove( perk );
	}

	public void OnComponentAdded( EntityComponent component )
	{
		if ( component is not Perk perk )
			return;

		if ( _list.Contains( perk ) )
			throw new System.Exception( "Trying to add to perks multiple times. This is gated by Entity:OnComponentAdded and should never happen!" );

		_list.Add( perk );
	}

	public void OnComponentRemoved( EntityComponent component )
	{
		if ( component is not Perk perk )
			return;

		_list.Remove( perk );
	}

	public IEnumerator<Perk> GetEnumerator() => _list.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
