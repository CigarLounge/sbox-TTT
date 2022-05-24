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

	public static void CacheGrenadeTypes()
	{
		var grenadeTypes = TypeLibrary.GetTypes<Grenade>();

		foreach ( var grenadeType in grenadeTypes )
		{
			var grenadeInfo = GameResource.GetInfo<CarriableInfo>( grenadeType );

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

		var grenade = TypeLibrary.Create<Grenade>( Rand.FromList( _cachedGrenadeTypes ) );
		if ( grenade is null )
			return;

		grenade.Position = Position + (Vector3.Up * GRENADE_DISTANCE_UP);
		grenade.Rotation = Rotation;
	}
}
