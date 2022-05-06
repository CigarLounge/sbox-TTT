using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

[Library( "ttt_grenade_random", Title = "Random Grenade" )]
[Hammer.EditorModel( "models/weapons/w_frag.vmdl" )]
public class RandomGrenade : Entity
{
	private static readonly List<Type> _cachedGrenadeTypes = new();
	private const int GRENADE_DISTANCE_UP = 4;

	static RandomGrenade()
	{
		var grenadeTypes = Library.GetAll<Grenade>();

		foreach ( var grenadeType in grenadeTypes )
		{
			var grenadeInfo = Asset.GetInfo<CarriableInfo>( Library.GetAttribute( grenadeType ).Name );

			if ( grenadeInfo is not null && grenadeInfo.Spawnable )
				_cachedGrenadeTypes.Add( grenadeType );
		}
	}

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;

		if ( _cachedGrenadeTypes.Count <= 0 )
			return;

		var grenade = Library.Create<Grenade>( Rand.FromList( _cachedGrenadeTypes ) );
		if ( grenade is null )
			return;

		grenade.Position = Position + (Vector3.Up * GRENADE_DISTANCE_UP);
		grenade.Rotation = Rotation;
	}
}
