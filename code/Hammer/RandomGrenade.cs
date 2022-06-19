using Sandbox;
using SandboxEditor;
using System;
using System.Collections.Generic;

namespace TTT;

[ClassName( "ttt_grenade_random" )]
[EditorModel( "models/weapons/w_frag.vmdl" )]
[HammerEntity]
[Title( "Random Grenade" )]
public class RandomGrenade : Entity
{
	private static readonly List<Type> _cachedGrenadeTypes = new();
	private const int GRENADE_DISTANCE_UP = 4;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;
		CacheGrenadeTypes();

		if ( _cachedGrenadeTypes.IsNullOrEmpty() )
			return;

		var grenade = TypeLibrary.Create<Grenade>( Rand.FromList( _cachedGrenadeTypes ) );
		if ( grenade is null )
			return;

		grenade.Position = Position + (Vector3.Up * GRENADE_DISTANCE_UP);
		grenade.Rotation = Rotation;
	}

	private static void CacheGrenadeTypes()
	{
		if ( !_cachedGrenadeTypes.IsNullOrEmpty() )
			return;

		var grenadeTypes = TypeLibrary.GetTypes<Grenade>();
		foreach ( var grenadeType in grenadeTypes )
		{
			var grenadeInfo = GameResource.GetInfo<CarriableInfo>( grenadeType );
			if ( grenadeInfo is not null && grenadeInfo.Spawnable )
				_cachedGrenadeTypes.Add( grenadeType );
		}
	}
}
